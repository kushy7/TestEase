using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestEase.Services;

namespace TestEase.ViewModels
{
    public partial class AboutPageViewModel: ObservableObject
    {
        private UpdateService _updateService;

        public AboutPageViewModel()
        {
            _updateService = new UpdateService();
            CheckForUpdatesCommand = new Command(async () => await CheckForUpdates());
        }

        public ICommand CheckForUpdatesCommand { get; }

        private async Task CheckForUpdates()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var isUpdateAvailable = _updateService.checkForUpdate(currentVersion);

            if (isUpdateAvailable)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Update Available!", "Download?", "Yes", "No");
            }
            else
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("No Update Available", "You are using the latest version.", "OK");
            }
        }
    }
}
