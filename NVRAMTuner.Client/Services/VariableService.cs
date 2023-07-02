namespace NVRAMTuner.Client.Services
{
    using CommunityToolkit.Mvvm.Messaging;
    using Interfaces;
    using Messages;
    using Messages.Variables;
    using Models;
    using Models.Nvram;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Renci.SshNet;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

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
        /// Instance of <see cref="IMessenger"/>
        /// </summary>
        private readonly IMessenger messenger;

        /// <summary>
        /// Dictionary holding the descriptions & default values of the
        /// firmware's NVRAM variables; keyed on the variable name.
        /// (see the NVRAMTuner.Scripts project to see where this data comes from)
        /// </summary>
        private readonly IDictionary<string, Tuple<string, string>> nvramDefaults;

        /// <summary>
        /// Initialises a new instance of the <see cref="VariableService"/> class
        /// </summary>
        /// <param name="networkService">An instance of <see cref="INetworkService"/></param>
        /// <param name="messenger">An instance of <see cref="IMessenger"/></param>
        public VariableService(INetworkService networkService, IMessenger messenger)
        {
            this.networkService = networkService;
            this.messenger = messenger;

            string rawFirmwareDefaults = ServiceResources.firmware_variable_defaults;
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
        /// <returns>An <see cref="Nvram"/> instance wrapped in an asynchronous
        /// <see cref="Task{TResult}"/></returns>
        public async Task<Nvram> GetNvramVariablesAsync()
        {
            SshCommand command =
                await this.networkService.RunCommandAgainstRouterAsync(ServiceResources.NVRAM_Show_Command);

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

            #endregion https://github.com/sshnet/SSH.NET/issues/1149

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
                int sizeBytes = parts[1].Length;
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

                // special cases
                if (parts[0] == VariableNames.NcSettingConf)
                {
                    NcSettingConf ncConf = new NcSettingConf
                    {
                        Name = name,
                        Value = new List<Tuple<string, string, string>>(),
                        Description = description,
                        DefaultValue = defaultValue,
                        SizeBytes = sizeBytes,
                        SpecialVariable = true
                    };

                    foreach (string component in parts[1].Split('<'))
                    {
                        if (!string.IsNullOrWhiteSpace(component))
                        {
                            string[] subComponents = component.Split('>');

                            ncConf.Value.Add(Tuple.Create(subComponents[0], subComponents[1], subComponents[2]));
                        }
                    }
                    
                    allVariables.Add(ncConf);
                }
                else
                {
                    NvramVariable variable = new NvramVariable
                    {
                        Name = name,
                        Value = parts[1],
                        Description = description,
                        DefaultValue = defaultValue,
                        SizeBytes = sizeBytes,
                    };

                    allVariables.Add(variable);
                }
            }

            this.messenger.Send(new LogMessage(new LogEntry
            {
                LogMessage = $"{allVariables.Count} variables loaded from router"
            }));

            Nvram nvram = new Nvram
            {
                Variables = allVariables,
                RetrievedAt = DateTime.Now,
                TotalSizeBytes = totalSizeBytes,
                RemainingSizeBytes = remainingSizeBytes
            };

            this.messenger.Send(new VariablesChangedMessage(nvram));

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
    }
}