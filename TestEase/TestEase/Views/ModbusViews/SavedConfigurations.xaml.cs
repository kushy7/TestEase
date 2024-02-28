using System;
using Microsoft.Maui.Controls;
using TestEase.ViewModels;
using TestEase.Services;

namespace TestEase.Views.ModbusViews;

public partial class SavedConfigurations : ContentView
{
    public SavedConfigurations()
    {
        InitializeComponent();
        // BindingContext = new SavedConfigurationsViewModel();
    }

    private void OpenFileExplorer(object sender, EventArgs e)
    {
        ConfigurationService s = new ConfigurationService();
        s.OpenConfigurationFolderInExplorer();
    }
}