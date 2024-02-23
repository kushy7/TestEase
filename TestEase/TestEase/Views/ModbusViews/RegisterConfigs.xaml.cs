namespace TestEase.Views.ModbusViews;
using TestEase.ViewModels;

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
            await viewModel.SaveConfigurationAsync(fileName);
        }
    }

    private async void SaveAs_Clicked(object sender, EventArgs e)
    {
        var viewModel = this.BindingContext as ModbusPageViewModel;
        if (viewModel != null)
        {
            // Using Application.Current.MainPage to access DisplayPromptAsync
            string defaultFileName = $"Configuration_{DateTime.Now:yyyyMMddHHmmss}.json";
            string fileName = await Application.Current.MainPage.DisplayPromptAsync("Save As", "Enter a file name:", initialValue: defaultFileName, accept: "Save", cancel: "Cancel");

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".json";
                }

                try
                {
                    await viewModel.SaveConfigurationAsync(fileName);
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
