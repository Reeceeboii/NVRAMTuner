namespace NVRAMTuner.Client.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Messages;
    using Models;
    using Services.Interfaces;
    using Services.Wrappers.Interfaces;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using Utils;

    /// <summary>
    /// ViewModel for the logs view
    /// </summary>
    public class LogsViewModel : ObservableObject
    {
        /// <summary>
        /// Instance of <see cref="IDialogService"/>
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// Instance of <see cref="IDataPersistenceService"/>
        /// </summary>
        private readonly IDataPersistenceService dataPersistenceService;

        /// <summary>
        /// Backing field for <see cref="LogEntries"/>
        /// </summary>
        private ObservableCollection<LogEntry> logEntries;

        /// <summary>
        /// The culture used for formatting of dates
        /// </summary>
        private readonly CultureInfo culture;

        /// <summary>
        /// Initialises a new instance of the <see cref="LogsViewModel"/> class
        /// </summary>
        /// <param name="messengerService">Instance of <see cref="IMessengerService"/></param>
        /// <param name="dialogService">Instance of <see cref="IDialogService"/></param>
        /// <param name="dataPersistenceService">Instance of <see cref="IDataPersistenceService"/></param>
        public LogsViewModel(
            IMessengerService messengerService, 
            IDialogService dialogService,
            IDataPersistenceService dataPersistenceService)
        {
            this.dialogService = dialogService;
            this.dataPersistenceService = dataPersistenceService;

            this.logEntries = new ObservableCollection<LogEntry>();
            this.culture = CultureInfo.CurrentCulture;

            this.NewLog(new LogEntry { LogMessage = "Logs have been initialised" });

            this.ClearLogsCommand = new RelayCommand(this.ClearLogsCommandHandler, this.ClearLogsCommandCanExecute);
            this.SaveLogsCommand = new RelayCommand(this.SaveLogsCommandHandler, this.SaveLogsCommandCanExecute);

            this.logEntries.CollectionChanged += (sender, args) =>
            {
                this.UpdateApplicableCommandsBasedOnLogChanges();
            };

            // register messages
            messengerService.Register<LogsViewModel, LogMessage>(this, 
                (recipient, message) => this.Receive(message));
        }

        /// <summary>
        /// Gets the command used to clear the logs list
        /// </summary>
        public RelayCommand ClearLogsCommand { get; }

        /// <summary>
        /// Gets the command used to save the current logs to a file
        /// </summary>
        public RelayCommand SaveLogsCommand { get; }

        /// <summary>
        /// Gets or sets an <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/> instances
        /// </summary>
        public ObservableCollection<LogEntry> LogEntries
        {
            get => this.logEntries;
            set => this.SetProperty(ref this.logEntries, value);
        }

        /// <summary>
        /// Gets a bool representing whether or not the logs can be cleared
        /// </summary>
        public bool CanClearLogs => this.ClearLogsCommand.CanExecute(null);

        /// <summary>
        /// Gets a bool representing whether or not the logs can be saved
        /// </summary>
        public bool CanSaveLogs => this.SaveLogsCommand.CanExecute(null);

        /// <summary>
        /// Adds a new log to the collection of logs
        /// </summary>
        /// <param name="entry">An instance of <see cref="LogEntry"/> to log</param>
        private void NewLog(LogEntry entry)
        {
            if (entry.LogTime == default)
            {
                entry.LogTime = DateTime.Now;
            }

            entry.PrettyLogTime = entry.LogTime.ToString(this.culture.DateTimeFormat.LongTimePattern);
            this.LogEntries.Add(entry);
        }

        /// <summary>
        /// Recipient method for a <see cref="LogMessage"/> message instance
        /// </summary>
        /// <param name="message">An instance of <see cref="LogMessage"/></param>
        public void Receive(LogMessage message)
        {
            this.NewLog(message.Value);
        }

        /// <summary>
        /// Method to handle the <see cref="ClearLogsCommand"/>
        /// </summary>
        private void ClearLogsCommandHandler()
        {
            this.LogEntries.Clear();
        }

        /// <summary>
        /// Method to determine if the <see cref="ClearLogsCommand"/> is able to execute
        /// </summary>
        /// <returns>A bool representing whether or not the command can be executed</returns>
        private bool ClearLogsCommandCanExecute()
        {
            return this.LogEntries.Any();
        }

        /// <summary>
        /// Method to handle the <see cref="SaveLogsCommand"/>
        /// </summary>
        private void SaveLogsCommandHandler()
        {
            string path = this.dialogService.ShowSaveAsDialog(
                "Text file (*.txt)|*.txt",
                $"NVRAMTuner logs {DateTime.Now.ToString(this.culture.DateTimeFormat.LongDatePattern)}.txt");

            if (path != string.Empty)
            {
                string logContent = StringUtils.LogEntryCollectionToString(this.LogEntries);
                this.dataPersistenceService.WriteTextToFile(path, logContent);
            }
        }
        
        /// <summary>
        /// Method to determine if the <see cref="SaveLogsCommand"/> is able to execute
        /// </summary>
        /// <returns>A bool representing whether or not the command can be executed</returns>
        private bool SaveLogsCommandCanExecute()
        {
            return this.LogEntries.Any();
        }

        /// <summary>
        /// When the logs change, update any relevant commands that care about it
        /// </summary>
        private void UpdateApplicableCommandsBasedOnLogChanges()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.ClearLogsCommand.NotifyCanExecuteChanged();
                this.SaveLogsCommand.NotifyCanExecuteChanged();
            });

            this.OnPropertyChanged(nameof(this.CanClearLogs));
            this.OnPropertyChanged(nameof(this.CanSaveLogs));
        }
    }
}