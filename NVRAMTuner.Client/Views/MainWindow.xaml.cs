﻿namespace NVRAMTuner.Client.Views
{
    using MahApps.Metro.Controls;
    using Microsoft.Extensions.DependencyInjection;
    using System.ComponentModel;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindow"/> class
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).ServiceContainer.Services.GetService<MainWindowViewModel>();
        }
    }
}
