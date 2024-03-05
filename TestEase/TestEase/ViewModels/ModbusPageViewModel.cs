using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using TestEase.Models;
using TestEase.Services;
using TestEase.Views.ModbusViews;
using static EasyModbus.ModbusServer;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace TestEase.ViewModels
{
    public partial class ModbusPageViewModel: ObservableObject
    {

        private ModbusServerModel _selectedServer;

        public ModbusServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    OnPropertyChanged(nameof(SelectedServer));
                    //if (CurrentItems != null)
                    //{
                    //    UpdateRegisterCollections();
                    //}
                    
                }
            }
        }


        public void CreateNewConfiguration()
        {
            //Reset the current server's configuration
            SelectedServer.WorkingConfiguration = new ConfigurationModel("new config");
        }

        public async Task SaveConfigurationAsync(string fileName)
        {
            SelectedServer.WorkingConfiguration.Name = fileName;
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = { new StringEnumConverter() }
            };

            string json = JsonConvert.SerializeObject(this.SelectedServer.WorkingConfiguration, settings);

            // Get the path to the user's Documents directory
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Combine the documents path with the filename to get the full path
            var filePath = System.IO.Path.Combine(documentsPath, fileName);

            // Write the JSON to the file
            await System.IO.File.WriteAllTextAsync(filePath, json);
       
        }

        public async Task SaveConfigurationAsAsync(string fileName)
        {
            // Adjusting to use the Documents directory
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            var json = JsonConvert.SerializeObject(SelectedServer.WorkingConfiguration);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task LoadConfigurationAsync(string fileName)
        {
            // Adjusting to use the Documents directory
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                SelectedServer.WorkingConfiguration = JsonConvert.DeserializeObject<ConfigurationModel>(json);
            }
        }



        public class SavedConfigurationsViewModel : INotifyPropertyChanged
        {
            private ObservableCollection<string> _configurationFiles;
            public ObservableCollection<string> ConfigurationFiles
            {
                get => _configurationFiles;
                set
                {
                    _configurationFiles = value;
                    OnPropertyChanged(nameof(ConfigurationFiles));
                }
            }

            private async Task LoadConfigurationsAsync()
            {
                // Specify the folder where the configuration files are saved
                //string configFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var s = new ConfigurationService();
                string configFolderPath = s.GetFolderPath();

                var files = Directory.EnumerateFiles(configFolderPath, "*.json");
                foreach (var file in files)
                {
                    ConfigurationFiles.Add(Path.GetFileName(file));
                }
            }

            public SavedConfigurationsViewModel()
            {
                ConfigurationFiles = new ObservableCollection<string>();
                LoadConfigurationsAsync();
            }

            

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // -----------------------
        // ENTRY TEXTS

        private string _fixedEntryText;
        public string FixedEntryText
        {
            get => _fixedEntryText;
            set
            {
                if (_fixedEntryText != value)
                {
                    _fixedEntryText = value;
                    OnPropertyChanged(nameof(FixedEntryText));
                }
            }
        }

        private string _lowerRangeText;
        public string LowerRangeText
        {
            get => _lowerRangeText;
            set
            {
                if (_lowerRangeText != value)
                {
                    _lowerRangeText = value;
                    OnPropertyChanged(nameof(LowerRangeText));
                }
            }
        }

        private string _upperRangeText;
        public string UpperRangeText
        {
            get => _upperRangeText;
            set
            {
                if (_upperRangeText != value)
                {
                    _upperRangeText = value;
                    OnPropertyChanged(nameof(UpperRangeText));
                }
            }
        }

        private string _startValText;
        public string StartValText
        {
            get => _startValText;
            set
            {
                if (_startValText != value)
                {
                    _startValText = value;
                    OnPropertyChanged(nameof(StartValText));
                }
            }
        }

        private string _endValText;
        public string EndValText
        {
            get => _endValText;
            set
            {
                if (_endValText != value)
                {
                    _endValText = value;
                    OnPropertyChanged(nameof(EndValText));
                }
            }
        }

        private string _periodText;
        public string PeriodText
        {
            get => _periodText;
            set
            {
                if (_periodText != value)
                {
                    _periodText = value;
                    OnPropertyChanged(nameof(PeriodText));
                }
            }
        }

        private bool _isFloatChecked;
        public bool IsFloatChecked
        {
            get => _isFloatChecked;
            set
            {
                if (_isFloatChecked != value)
                {
                    _isFloatChecked = value;
                    OnPropertyChanged(nameof(IsFloatChecked));
                }
            }
        }

        public AppViewModel AppViewModel { get; }
        public ModbusPageViewModel(AppViewModel appViewModel)
        {
            AppViewModel = appViewModel;

            Trace.WriteLine("Started server"); // DELETE
            SelectedServer = new ModbusServerModel(502);
            SelectedServer.StartServer();
            SelectedServer.IsRunning = true;
            AppViewModel.ModbusServers.Add(SelectedServer);

            LoadConfigurations();

        }

        public async Task LoadConfigurations()
        {
            var service = new ConfigurationService();

            string folderPath = service.GetFolderPath();
            string[] files = Directory.GetFiles(folderPath, "*.json");

            foreach (string file in files)
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(file);
                    var config = System.Text.Json.JsonSerializer.Deserialize<ConfigurationModel>(jsonContent);

                    if (config != null)
                    {
                        // await Application.Current.MainPage.DisplayAlert("Error", $"Name: {config.Name}\nRegisters: {config.RegisterModels.Count}", "OK");
                        AppViewModel.Configurations.Add(config);
                    }
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    // Handle JSON-specific exceptions, e.g., malformed JSON
                    Debug.WriteLine($"JSON Error: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions, e.g., file read errors
                    Debug.WriteLine($"Error loading configuration from file {file}: {ex.Message}");
                }
            }
        }
    }
}
