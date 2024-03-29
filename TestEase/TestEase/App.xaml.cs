using Microsoft.Maui.Platform;
using System.Diagnostics;
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

        //protected override Window CreateWindow(IActivationState activationState)
        //{
        //    var window = base.CreateWindow(activationState);

        //    const int newWidth = 1920;
        //    const int newHeight = 1080;

        //    window.Width = newWidth;
        //    window.Height = newHeight;

        //    window.X = 0;
        //    window.Y = 0;

        //    return window;
        //}
    }
}
