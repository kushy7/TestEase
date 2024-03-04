namespace TestEase.Views.ModbusViews;
using TestEase.ViewModels;
using TestEase.Services;
using TestEase.Models;

public partial class RegisterConfigs : ContentView
{
    public RegisterConfigs()
    {
        InitializeComponent();
    }

    private void New_Clicked(object sender, EventArgs e)
    {
        // Assuming your ViewModel is bound as the DataContext or BindingContext
        var viewModel = this.BindingContext as ModbusPageViewModel;
        viewModel?.CreateNewConfiguration();
        viewModel?.SelectedServer.ResetRegistersToDefault();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (viewModel != null)
        {
            string fileName = "defaultConfig.json"; // Define how you determine the file name
            try
            {
                await viewModel.SaveConfigurationAsync(fileName);
                // Assuming saving was successful, notify the user
                await Application.Current.MainPage.DisplayAlert("Success", "Configuration saved as " + fileName, "OK");
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
                    await Application.Current.MainPage.DisplayAlert("Success", "Configuration saved successfully.", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save the configuration: {ex.Message}", "OK");
                }
            }
        }
    }



}
