namespace NVRAMTuner.Client.Messages.Variables
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using Models.Nvram;

    /// <summary>
    /// Request message, requesting the currently selected variable from the list of
    /// staged variables
    /// </summary>
    public class RequestSelectedStagedVariableMessage : RequestMessage<IVariable>
    {
    }
}