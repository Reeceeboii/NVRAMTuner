namespace NVRAMTuner.Client.Messages.Variables.Staged
{
    using CommunityToolkit.Mvvm.Messaging.Messages;
    using ViewModels.Variables;

    /// <summary>
    /// Request message that can be used to request the current number of staged variables
    /// from the <see cref="StagedChangesViewModel"/>
    /// </summary>
    public class RequestNumOfStagedVariablesMessage : RequestMessage<int>
    {
    }
}