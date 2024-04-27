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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public string AppVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"Version {version.Major}.{version.Minor}.{version.Build}";
            }
        }


        private async Task CheckForUpdates()
        {

            if (_updateService.isUpdateAvailable())
            {
                string version = _updateService.getGitHubReleaseVersion();
                bool ans = await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Update Available!", $"There is a new version {version} of TestEase available, click the 'Update' button to install.", "Later", "Update");
                if (!ans)
                {
                    await _updateService.DownloadGitHubReleaseAsset(_updateService.getAssetUrl());
                    await _updateService.DownloadUpdater();
                    _updateService.ExtractZipFile();
                    _updateService.performUpdate();

                }
            }
            else
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("No Update Available", "You are using the latest version.", "OK");
            }
        }
    }
}
