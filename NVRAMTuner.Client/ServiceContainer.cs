namespace NVRAMTuner.Client
{
    using Microsoft.Extensions.DependencyInjection;
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

            // Services
            //collection.AddScoped<IProcessService, ProcessService>();

            // ViewModels
            collection.AddScoped<MainWindowViewModel>();

            this.Services = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets an instance of <see cref="IServiceProvider"/>
        /// </summary>
        public IServiceProvider Services { get; }
    }
}