using System;
using Microsoft.Maui.Controls;
using TestEase.ViewModels;
using TestEase.Services;
using TestEase.Models;
using System.Xml.Linq;

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

    //triggers a new config to be loaded into the menu
    private async void OnOpenClicked(object sender, EventArgs e)
    {
        var vm = this.BindingContext as ModbusPageViewModel;

        if (vm.SelectedServer.IsNotSaved)
        {
            bool isUserSure = await Application.Current.MainPage.DisplayAlert("Confirmation", "Your working configuration has not yet been saved, continue?", "Yes", "No");
            if (!isUserSure)
                return;
        }

        var menuItem = (MenuItem)sender;
        var item = (ConfigurationModel) menuItem.BindingContext;

        //clear and populate the new config
        if (item != null)
        {
            vm.SelectedServer.clearServerRegisters();

            ConfigurationModel copyOfItem = item.DeepCopy();
            vm.SelectedServer.WorkingConfiguration = copyOfItem;

            vm.SelectedServer.UpdateRegisterCollections();
            vm.SelectedServer.SelectedRegister = null;
            vm.SelectedServer.IsNotSaved = false;
        }
    }
}