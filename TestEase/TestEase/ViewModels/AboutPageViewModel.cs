using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (_updateService.isUpdateAvailable())
            {
                bool ans = await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Update Available!", "There is a new version of TestEase available, click the 'Update' button to install.", "Later", "Update");
                if (!ans)
                {
                    await _updateService.DownloadGitHubReleaseAsset(_updateService.getAssetUrl());
                    _updateService.ExtractZipFile();
                    _updateService.OpenExeFile();
                }
            }
            else
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("No Update Available", "You are using the latest version.", "OK");
            }
        }
    }
}
