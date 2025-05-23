﻿using Microsoft.UI.Xaml;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestEase.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// Text="{Binding Path=VersionNumber, Source={x:Static local:App.Current}}"

        public static string VersionNumber { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public App()
        {
            this.InitializeComponent();

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
