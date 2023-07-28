namespace NVRAMTuner.Client
{
    using CommunityToolkit.Mvvm.Messaging;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Services.Interfaces;
    using Services.Network;
    using Services.Network.Interfaces;
    using System;
    using System.IO.Abstractions;
    using ViewModels;
    using ViewModels.Variables;

    /// <summary>
    /// DI (IoC) container for services/ViewModels etc...
    /// </summary>
    public class ServiceContainer
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ServiceContainer"/> class
        /// </summary>
        public ServiceContainer()
        {
            ServiceCollection collection = new ServiceCollection();

            // Misc
            collection.AddScoped<IFileSystem, FileSystem>(); // System.IO.Abstractions
            collection.AddScoped<IDialogCoordinator, DialogCoordinator>();
            collection.AddScoped<IMessenger, WeakReferenceMessenger>();

            // Services
            collection.AddScoped<IProcessService, ProcessService>();
            collection.AddScoped<IEnvironmentService, EnvironmentService>();
            collection.AddScoped<IDialogService, DialogService>();
            collection.AddScoped<INetworkService, NetworkService>();
            collection.AddScoped<IDataPersistenceService, DataPersistenceService>();
            collection.AddScoped<IDataEncryptionService, DataEncryptionService>();
            collection.AddScoped<IWindowsSecurityService, WindowsSecurityService>();
            collection.AddScoped<IVariableService, VariableService>();
            collection.AddSingleton<ISettingsService, SettingsService>();

            // ViewModels and navigation
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<AboutWindowViewModel>();
            collection.AddScoped<HomeViewModel>();
            collection.AddScoped<LogsViewModel>();
            collection.AddTransient<RouterSetupViewModel>();
            collection.AddScoped<VariablesViewModel>();
            collection.AddScoped<EditsViewModel>();
            collection.AddScoped<SettingsFlyoutViewModel>();
            collection.AddScoped<StagedChangesViewModel>();

            this.Services = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets an instance of <see cref="IServiceProvider"/>
        /// </summary>
        public IServiceProvider Services { get; }
    }
}