using Microsoft.Maui.Platform;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using TestEase.Models;
using TestEase.Services;
using TestEase.ViewModels;

namespace TestEase
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //Application.Current.UserAppTheme = AppTheme.Light;
            var appViewModel = new AppViewModel();

            var modbusService = serviceProvider.GetService<ModbusService>();
            modbusService.StartPeriodicUpdate(TimeSpan.FromSeconds(1));

            // Load the theme preference
            var themePreference = Preferences.Get("AppTheme", "Light");
            Application.Current.UserAppTheme = themePreference == "Dark" ? AppTheme.Dark : AppTheme.Light;

            MainPage = new AppShell() { BindingContext = appViewModel };
            MainPage.Title = $"Test Ease v{Assembly.GetExecutingAssembly().GetName().Version}";

            var updateService = serviceProvider.GetService<UpdateService>();
            updateService.checkForUpdate();

        }


        public async Task LoadConfigurations()
        {
            var service = new ConfigurationService();
            var appViewModel = this.MainPage.BindingContext as AppViewModel;

            string folderPath = service.GetFolderPath();
            string[] files = Directory.GetFiles(folderPath, "*.json");

            foreach ( string file in files )
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(file);
                    ConfigurationModel config = JsonSerializer.Deserialize<ConfigurationModel>(jsonContent);
                    if (config != null)
                    {
                        appViewModel.Configurations.Add(config);
                    }
                }
                catch (JsonException jsonEx)
                {
                    // Handle JSON-specific exceptions, e.g., malformed JSON
                    await Current.MainPage.DisplayAlert("Error", $"JSON Error from file {file}: {jsonEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions, e.g., file read errors
                    await Current.MainPage.DisplayAlert("Error", $"Error loading configuration from file {file}: {ex.Message}", "OK");
                }
            }
        }
    }
}
