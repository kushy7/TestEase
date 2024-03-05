namespace TestEase.Views.ModbusViews;
using TestEase.ViewModels;
using TestEase.Services;
using TestEase.Models;
using System.Xml.Linq;

public partial class RegisterConfigs : ContentView
{
    public RegisterConfigs()
    {
        InitializeComponent();
    }

    private async void New_Clicked(object sender, EventArgs e)
    {
        // Assuming your ViewModel is bound as the DataContext or BindingContext
        var viewModel = this.BindingContext as ModbusPageViewModel;

        if (viewModel.SelectedServer.IsNotSaved)
        {
            bool isUserSure = await Application.Current.MainPage.DisplayAlert("Confirmation", "Your working configuration has not yet been saved, continue?", "Yes", "No");
            if (!isUserSure)
                return;
        }

        viewModel?.CreateNewConfiguration();
        viewModel.SelectedServer.SelectedRegister = null;
        viewModel.SelectedServer.ResetRegistersToDefault();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (viewModel != null)
        {
            string name = viewModel.SelectedServer.WorkingConfiguration.Name; // Define how you determine the file name
            try
            {

                ConfigurationService s = new ConfigurationService();
                var duplicate = viewModel.AppViewModel.Configurations.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
                if (duplicate != null)
                {
                    viewModel.AppViewModel.Configurations.Remove(duplicate);
                }

                viewModel.SelectedServer.WorkingConfiguration.Name = name; // update name of config object
                await s.SaveConfigurationAsync(viewModel.SelectedServer.WorkingConfiguration, name + ".json"); // save json to directory
                ConfigurationModel newConfig = viewModel.SelectedServer.WorkingConfiguration.DeepCopy();

                viewModel.AppViewModel.Configurations.Add(newConfig); // add to global list

                // Assuming saving was successful, notify the user
                viewModel.SelectedServer.IsNotSaved = false;
                await Application.Current.MainPage.DisplayAlert("Success", "Configuration " + name + " saved", "OK");
            }
            catch (Exception ex)
            {
                // If there's an error during save, notify the user
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save the configuration: {ex.Message}", "OK");
            }
        }
    }


    private async void SaveAs_Clicked(object sender, EventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (viewModel != null)
        {
            string fileName = (await Application.Current.MainPage.DisplayPromptAsync("Save As", "Enter a file name:", initialValue: viewModel.SelectedServer.WorkingConfiguration.Name, accept: "Save", cancel: "Cancel")).ToString();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".json";
                }
                try
                {
                    // Check for existing configuration with given name, ignoring caps
                    string name = fileName.Remove(fileName.Length - 5);
                    var duplicate = viewModel.AppViewModel.Configurations.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());
                    if (duplicate != null)
                    {
                        bool isUserSure = await Application.Current.MainPage.DisplayAlert("Confirmation", $"A configuration with name {name} already exists, continue?", "Yes", "No");
                        if (isUserSure)
                        {
                            viewModel.AppViewModel.Configurations.Remove(duplicate);
                        } else
                        {
                            return;
                        }
                    }
                    // viewModel.SelectedServer.WorkingConfiguration.Name = name; // update name of config object
                    ConfigurationService s = new ConfigurationService();

                    viewModel.SelectedServer.WorkingConfiguration.Name = name; // update name of config object
                    await s.SaveConfigurationAsync(viewModel.SelectedServer.WorkingConfiguration, fileName); // save json to directory
                    ConfigurationModel newConfig = viewModel.SelectedServer.WorkingConfiguration.DeepCopy();

                    viewModel.AppViewModel.Configurations.Add(newConfig); // add to global list

                    viewModel.SelectedServer.IsNotSaved = false;
                    await Application.Current.MainPage.DisplayAlert("Success", "Configuration saved as " + name, "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save the configuration: {ex.Message}", "OK");
                }
            }
        }
    }



}
