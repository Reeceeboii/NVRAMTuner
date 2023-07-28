namespace NVRAMTuner.Client.Events
{
    using Services.Network.Interfaces;
    using System;

    /// <summary>
    /// Custom event args for the event fired by classes implementing <see cref="INetworkService"/>.
    /// Specifically, <see cref="INetworkService.ConnectionTimerSecondTick"/>
    /// </summary>
    public class ActiveConnectionTimerTickArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current elapsed time of the timer
        /// </summary>
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// A pretty (and UI ready) string representation of <see cref="Elapsed"/>.
        /// In HH:MM:SS format.
        /// </summary>
        public string ElapsedPretty { get; set; }
    }
}