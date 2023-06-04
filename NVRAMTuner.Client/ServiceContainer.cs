﻿namespace NVRAMTuner.Client
{
    using CommunityToolkit.Mvvm.Messaging;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Services.Interfaces;
    using System;
    using System.IO.Abstractions;
    using ViewModels;

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
            collection.AddScoped<IFileSystem, FileSystem>();
            collection.AddScoped<IDialogCoordinator, DialogCoordinator>();
            collection.AddScoped<IMessenger, WeakReferenceMessenger>();

            // Services
            collection.AddScoped<IProcessService, ProcessService>();
            collection.AddScoped<IEnvironmentService, EnvironmentService>();
            collection.AddScoped<ISshClientService, SshClientService>();
            collection.AddScoped<IDialogService, DialogService>();
            collection.AddScoped<INetworkService, NetworkService>();

            // ViewModels and navigation
            collection.AddScoped<MainWindowViewModel>();
            collection.AddScoped<AboutWindowViewModel>();
            collection.AddScoped<MenuViewModel>();
            collection.AddScoped<HomeViewModel>();
            collection.AddScoped<RouterSetupViewModel>();

            this.Services = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets an instance of <see cref="IServiceProvider"/>
        /// </summary>
        public IServiceProvider Services { get; }
    }
}