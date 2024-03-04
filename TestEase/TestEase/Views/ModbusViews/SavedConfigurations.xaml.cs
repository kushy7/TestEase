using System;
using Microsoft.Maui.Controls;
using TestEase.ViewModels;
using TestEase.Services;
using TestEase.Models;

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

    private void OnOpenClicked(object sender, EventArgs e)
    {
        var vm = this.BindingContext as ModbusPageViewModel;
        var menuItem = (MenuItem)sender;
        var item = (ConfigurationModel) menuItem.BindingContext;

        if (item != null)
        {
            vm.SelectedServer.clearServerRegisters();

            ConfigurationModel copyOfItem = item.DeepCopy();
            vm.SelectedServer.WorkingConfiguration = copyOfItem;

            vm.SelectedServer.UpdateRegisterCollections();
            vm.SelectedServer.SelectedRegister = null;
        }
    }
}