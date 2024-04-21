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
        //server model that is currently in use
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
                    
                }
            }
        }

        //Sets the working configuration for the server in use to a new config model
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


        // sets and gets the configuration files to be viewed
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


        // Lower entry text value for handling a linear lower bound
        private string _linearLowerEntryText;
        public string LinearLowerEntryText
        {
            get => _linearLowerEntryText;
            set
            {
                if (_linearLowerEntryText != value)
                {
                    _linearLowerEntryText = value;
                    OnPropertyChanged(nameof(LinearLowerEntryText));
                }
            }
        }

        // Upper entry text value for handling a linear upper bound
        private string _linearUpperEntryText;
        public string LinearUpperEntryText
        {
            get => _linearUpperEntryText;
            set
            {
                if (_linearUpperEntryText != value)
                {
                    _linearUpperEntryText = value;
                    OnPropertyChanged(nameof(LinearUpperEntryText));
                }
            }
        }

        // Increment entry text value for handling how much you want to increment between the bounds by with
        // each step
        private string _linearIncrementEntryText;
        public string LinearIncrementEntryText
        {
            get => _linearIncrementEntryText;
            set
            {
                if (_linearIncrementEntryText != value)
                {
                    _linearIncrementEntryText = value;
                    OnPropertyChanged(nameof(LinearIncrementEntryText));
                }
            }
        }

        // fixed value entry text 
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

        //lower range value entry text for random inputs
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

        //upper range value entry text for random inputs
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
        //start value entry text for curve inputs
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
        //end value entry text for curve inputs
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

        //period entry value text for curve input 
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

        private bool  _isFixedSelected = false;
        public bool IsFixedSelected
        {
            get => _isFixedSelected;
            set
            {
                if (_isFixedSelected != value)
                {
                    _isFixedSelected = value;
                    OnPropertyChanged(nameof(IsFixedSelected));
                }
            }
        }

        private bool _isRangeSelected = true;
        public bool IsRangeSelected
        {
            get => _isRangeSelected;
            set
            {
                if (_isRangeSelected != value)
                {
                    _isRangeSelected = value;
                    OnPropertyChanged(nameof(IsRangeSelected));
                }
            }
        }

        public AppViewModel AppViewModel { get; }
        public ModbusPageViewModel(AppViewModel appViewModel)
        {
            AppViewModel = appViewModel;

            if (AppViewModel.ModbusServers.Count == 0)
            {
                SelectedServer = new ModbusServerModel(502);
                SelectedServer.StartServer();
                SelectedServer.IsRunning = true;
                AppViewModel.ModbusServers.Add(SelectedServer);
            } else
            {
                SelectedServer = AppViewModel.ModbusServers[0];
            }
            


            LoadConfigurations();

        }
        //loads all configurations from my documents to be loaded in later 
        public async Task LoadConfigurations()
        {
            //creates a new configuration service
            var service = new ConfigurationService();
            //gets the folder path from the service, mydocuments
            string folderPath = service.GetFolderPath();
            //gets all the files from the my documents path and saves them to a list of strings
            string[] files = Directory.GetFiles(folderPath, "*.json");
            //takes each file and adds configs to configurations to be pulled from later
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
