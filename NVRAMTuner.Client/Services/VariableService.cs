namespace NVRAMTuner.Client.Services
{
    using Interfaces;
    using Messages;
    using Messages.Variables;
    using Models.Nvram;
    using Models.Nvram.Concrete;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Renci.SshNet;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Wrappers.Interfaces;

    /// <summary>
    /// Service to handle NVRAM variable operations
    /// </summary>
    public class VariableService : IVariableService
    {
        /// <summary>
        /// Instance of <see cref="INetworkService"/>
        /// </summary>
        private readonly INetworkService networkService;

        /// <summary>
        /// Instance of <see cref="IMessengerService"/>
        /// </summary>
        private readonly IMessengerService messengerService;

        /// <summary>
        /// Dictionary holding the descriptions and default values of the
        /// firmware's NVRAM variables; keyed on the variable name.
        /// (see the NVRAMTuner.Scripts project to see where this data comes from)
        /// </summary>
        private readonly IDictionary<string, Tuple<string, string>> nvramDefaults;

        /// <summary>
        /// Initialises a new instance of the <see cref="VariableService"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="messengerService">An instance of <see cref="IMessengerService"/></param>
        public VariableService(INetworkService networkService, IMessengerService messengerService)
        {
            this.networkService = networkService;
            this.messengerService = messengerService;

            string rawFirmwareDefaults = ServiceResources.FirmwareVariableDefaults;
            JObject defaultJsonObject = JsonConvert.DeserializeObject<JObject>(rawFirmwareDefaults);

            this.nvramDefaults = new Dictionary<string, Tuple<string, string>>();

            // insert variables and defaults/descriptions into the dictionary
            foreach (JProperty prop in defaultJsonObject.Properties())
            {
                this.nvramDefaults[prop.Name] =
                    Tuple.Create((string)prop.Value["description"], (string)prop.Value["default"]);
            }
        }

        /// <summary>
        /// Loads all NVRAM variables by contacting the router and running the 'nvram show' command.
        /// Returns this data in an <see cref="Nvram"/> variable
        /// </summary>
        /// <returns>An asynchronous<see cref="Task{TResult}"/></returns>
        public async Task<Nvram> GetNvramVariablesAsync()
        {
            SshCommand command =
                await this.networkService.RunCommandAgainstRouterAsync(ServiceResources.NvramShowCommand);

            #region https://github.com/sshnet/SSH.NET/issues/1149

            // This is ghastly. But for some reason this issue exists (or I'm being dense)
            const string pattern = @"size:\s(\d+)\sbytes\s\((\d+)\sleft\)";
            Match m = Regex.Match(command.Error, pattern);

            int totalSizeBytes = 0;
            int remainingSizeBytes = 0;
            
            if (m.Success)
            {
                totalSizeBytes = int.Parse(m.Groups[1].Value);
                remainingSizeBytes = int.Parse(m.Groups[2].Value);
            }

            #endregion

            List<IVariable> allVariables = new List<IVariable>();

            foreach (string line in command.Result.Split('\n').ToList())
            {
                if (!line.Contains("="))
                {
                    continue;
                }

                string[] parts = line.Split('=');

                // extract common pieces of information
                string name = parts[0];
                string originalValue = parts[1];
                int sizeBytes = originalValue.Length;
                string description = this.GetDescriptionForVariable(name);
                string defaultValue = this.GetDefaultValueForVariable(name);

                if (string.IsNullOrEmpty(description))
                {
                    description = "Unknown";
                }

                if (string.IsNullOrEmpty(defaultValue))
                {
                    defaultValue = "Unknown";
                }

                if (parts[0] == VariableNames.NcSettingConf)
                {
                    List<Tuple<string, string, string>> parsed 
                        = (List<Tuple<string, string, string>>)ParseAngleBracketVariable(parts[1], typeof(NcSettingConf));

                    NcSettingConf ncConf = new NcSettingConf
                    {
                        Name = name,
                        Value = parsed,
                        OriginalValue = originalValue,
                        ValueDelta = originalValue,
                        Description = description,
                        DefaultValue = defaultValue,
                        SizeBytes = sizeBytes,
                        SpecialVariable = true
                    };

                    allVariables.Add(ncConf);
                }
                else if (parts[0] == VariableNames.CustomClientList)
                {
                    List<Tuple<string, string, string, string, string, string>> parsed
                        = (List<Tuple<string, string, string, string, string, string>>)ParseAngleBracketVariable(
                            parts[1], typeof(CustomClientList));

                    CustomClientList ccl = new CustomClientList
                    {
                        Name = name,
                        Value = parsed,
                        OriginalValue = originalValue,
                        ValueDelta = originalValue,
                        Description = description,
                        DefaultValue = defaultValue,
                        SizeBytes = sizeBytes,
                        SpecialVariable = true
                    };

                    allVariables.Add(ccl);
                }
                else
                {
                    NvramVariable variable = new NvramVariable
                    {
                        Name = name,
                        Value = parts[1],
                        OriginalValue = originalValue,
                        ValueDelta = originalValue,
                        Description = description,
                        DefaultValue = defaultValue,
                        SizeBytes = sizeBytes,
                    };

                    allVariables.Add(variable);
                }
            }

            this.messengerService.Send(new LogMessage($"{allVariables.Count} variables loaded from router"));

            int totalVariableSizeBytes = allVariables.Sum(variable => variable.Name.Length + variable.OriginalValue.Length);

            Nvram nvram = new Nvram
            {
                Variables = allVariables,
                RetrievedAt = DateTime.Now,
                TotalSizeBytes = totalSizeBytes,
                RemainingSizeBytes = remainingSizeBytes,
                VariableSizeBytes = totalVariableSizeBytes
            };

            this.messengerService.Send(new VariablesChangedMessage(nvram));
            return nvram;
        }

        /// <summary>
        /// Attempts to retrieve the description for a given variable using its name.
        /// This indexes <see cref="nvramDefaults"/>, a dictionary of descriptions and default
        /// values created by the NVRAMTuner.Scripts project.
        /// </summary>
        /// <param name="variableName">The variable name to use in the lookup</param>
        /// <returns>A description, or "unknown"</returns>
        private string GetDescriptionForVariable(string variableName)
        {
            return this.nvramDefaults.TryGetValue(variableName, out Tuple<string, string> t)
                ? t.Item1
                : "Unknown";
        }

        /// <summary>
        /// Attempts to retrieve the default value for a given variable using its name.
        /// This indexes <see cref="nvramDefaults"/>, a dictionary of descriptions and default
        /// values created by the NVRAMTuner.Scripts project.
        /// </summary>
        /// <param name="variableName">The variable name to use in the lookup</param>
        /// <returns>A default value, or "unknown"</returns>
        private string GetDefaultValueForVariable(string variableName)
        {
            return this.nvramDefaults.TryGetValue(variableName, out Tuple<string, string> t)
                ? t.Item2
                : "Unknown";
        }

        /// <summary>
        /// Parses variables that follow the angle bracket format
        /// </summary>
        /// <param name="rawValue">The raw string representation of the variable</param>
        /// <param name="variableType">The type of <see cref="IVariable"/> to which the
        /// raw value should be converted to</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITuple"/> values</returns>
        private static IEnumerable<ITuple> ParseAngleBracketVariable(string rawValue, Type variableType)
        {
            if (!typeof(IVariable).IsAssignableFrom(variableType))
            {
                throw new ArgumentException($@"Does not implement {nameof(IVariable)}", nameof(variableType));
            }

            if (variableType.IsAssignableFrom(typeof(NcSettingConf)))
            {
                return ParseComponents(rawValue, subComponents
                    => Tuple.Create(subComponents[0], subComponents[1], subComponents[2]));
            }

            if (variableType.IsAssignableFrom(typeof(CustomClientList)))
            {
                return ParseComponents(rawValue, subComponents
                    => Tuple.Create(
                        subComponents[0],
                        subComponents[1],
                        subComponents[2],
                        subComponents[3],
                        subComponents[4],
                        subComponents[5]));
            }

            return new List<ITuple>();
        }

        /// <summary>
        /// Given a raw string value, this method accepts a delegate function that defines how to convert
        /// the value into type T after splitting the value, first on
        /// the less than character, and then on the greater than character.
        /// </summary>
        /// <typeparam name="T">The required resultant type of the parsing operation on the components</typeparam>
        /// <param name="rawValue">The raw string value, that is to be broken into sub-components</param>
        /// <param name="func">A delegate defining how to convert the sub-components into TS</param>
        /// <returns>A <see cref="List{T}"/></returns>
        private static List<T> ParseComponents<T>(string rawValue, Func<string[], T> func)
        {
            return (
                from component in rawValue.Split('<') 
                where !string.IsNullOrWhiteSpace(component) 
                select component.Split('>') 
                into subComponents 
                select func(subComponents)).ToList();
        }
    }
}