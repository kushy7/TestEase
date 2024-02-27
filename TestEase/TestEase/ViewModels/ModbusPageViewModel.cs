using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TestEase.Models;
using TestEase.Services;
using TestEase.Views.ModbusViews;
using static EasyModbus.ModbusServer;

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
                string configFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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




        //public ObservableCollection<IRegister> DiscreteInputs { get; set; } = new();
        //public ObservableCollection<IRegister> Coils { get; set; } = new();
        //public ObservableCollection<IRegister> InputRegisters { get; set; } = new();
        //public ObservableCollection<IRegister> HoldingRegisters { get; set; } = new();

        //private ObservableCollection<IRegister> _currentItems;
        //public ObservableCollection<IRegister> CurrentItems
        //{
        //    get => _currentItems;
        //    set
        //    {
        //        _currentItems = value;
        //        OnPropertyChanged(nameof(CurrentItems));
        //    }
        //}

        //private IRegister _selectedRegister;
        //public IRegister SelectedRegister
        //{
        //    get => _selectedRegister;
        //    set
        //    {
        //        if (_selectedRegister != value)
        //        {
        //            _selectedRegister = value;
        //            OnPropertyChanged(nameof(SelectedRegister));
        //            // Update IsRegisterSelected whenever SelectedRegister changes
        //            IsRegisterSelected = _selectedRegister?.RegisterType == RegisterType.HoldingRegister ||
        //                                        _selectedRegister?.RegisterType == RegisterType.InputRegister;

        //            // Update IsBooleanRegisterSelected based on the RegisterType
        //            IsBooleanRegisterSelected = _selectedRegister?.RegisterType == RegisterType.DiscreteInput ||
        //                                        _selectedRegister?.RegisterType == RegisterType.Coil;
        //        }
        //    }
        //}

        //// Add IsRegisterSelected property to indicate if a register is selected
        //private bool _isRegisterSelected;
        //public bool IsRegisterSelected
        //{
        //    get => _isRegisterSelected;
        //    set => SetProperty(ref _isRegisterSelected, value);
        //}

        //// Add IsBooleanRegisterSelected to indicate if a register is of discrete input or coil type
        //private bool _isBooleanRegisterSelected;
        //public bool IsBooleanRegisterSelected
        //{
        //    get => _isBooleanRegisterSelected;
        //    set => SetProperty(ref _isBooleanRegisterSelected, value);
        //}


        //private bool _selectedBooleanValue;
        //public bool SelectedBooleanValue
        //{
        //    get => _selectedBooleanValue;
        //    set
        //    {
        //        if (_selectedBooleanValue != value)
        //        {
        //            _selectedBooleanValue = value;
        //            OnPropertyChanged(); 
        //        }
        //    }
        //}


        //private bool _isFloatConfigurationChecked;
        //public bool IsFloatConfigurationChecked
        //{
        //    get => _isFloatConfigurationChecked;
        //    set
        //    {
        //        if (_isFloatConfigurationChecked != value)
        //        {
        //            _isFloatConfigurationChecked = value;
        //            OnPropertyChanged(nameof(IsFloatConfigurationChecked));

        //        }
        //    }
        //}

        //private string _currentTabName = "HoldingRegisters"; // Default tab
        //public string CurrentTabName
        //{
        //    get => _currentTabName;
        //    set
        //    {
        //        if (_currentTabName != value)
        //        {
        //            _currentTabName = value;
        //            OnPropertyChanged(nameof(CurrentTabName));
        //        }
        //    }
        //}

        //private IEnumerable<IRegister> GetCurrentTabCollection()
        //{
        //    return CurrentTabName switch
        //    {
        //        "DiscreteInputs" => DiscreteInputs,
        //        "Coils" => Coils,
        //        "InputRegisters" => InputRegisters,
        //        "HoldingRegisters" => HoldingRegisters,
        //        _ => Enumerable.Empty<IRegister>(),
        //    };
        //}

        private string _fixedNonFloatEntryText;

        public string FixedNonFloatEntryText
        
        {
            get => _fixedNonFloatEntryText;
            set
            {
                if (_fixedNonFloatEntryText != value)
                {
                    _fixedNonFloatEntryText = value;
                    OnPropertyChanged(nameof(FixedNonFloatEntryText));
                }
            }
        }

        public AppViewModel AppViewModel { get; }

        private readonly ModbusService _service;
        public ModbusPageViewModel(AppViewModel appViewModel)
        {
            _service = new ModbusService(appViewModel);
            AppViewModel = appViewModel;

            Trace.WriteLine("Started server"); // DELETE
            SelectedServer = new ModbusServerModel(502);
            SelectedServer.StartServer();
            SelectedServer.IsRunning = true;
            AppViewModel.ModbusServers.Add(SelectedServer);


            // To update these in the frontend, consider making these objects and storing all this these observables
            // inside the ModbusServerModel. This would give the ModbusService access through the appViewModel.
            //for (int i = 1; i < 65535; i++)
            //{
            //    DiscreteInputs.Add(new Register<bool>
            //    {
            //        Address = i,
            //        Value = false,
            //        Name = "",
            //        RegisterType = RegisterType.DiscreteInput
            //    });
            //    Coils.Add(new Register<bool>
            //    {
            //        Address = i,
            //        Value = false,
            //        Name = "",
            //        RegisterType = RegisterType.Coil
            //    });
            //    InputRegisters.Add(new Register<short>
            //    {
            //        Address = i,
            //        Value = 0,
            //        Name = "",
            //        RegisterType = RegisterType.InputRegister
            //    });
            //    HoldingRegisters.Add(new Register<short>
            //    {
            //        Address = i,
            //        Value = 0,
            //        Name = "",
            //        RegisterType = RegisterType.HoldingRegister
            //    });

            //    // Holding Registers by default
            //    CurrentItems = HoldingRegisters;
            //}

        }

        //private void UpdateRegisterCollections()
        //{
        //    // Assuming you want to clear the existing collections and repopulate them
        //    //DiscreteInputs.Clear();
        //    //Coils.Clear();
        //    //InputRegisters.Clear();
        //    //HoldingRegisters.Clear();
        //    Trace.WriteLine("Updating Register collections"); // DELETE

        //    CurrentItems = null;

        //    for (int i = 1; i < 65535; i++)
        //    {
        //        DiscreteInputs[i - 1] = (new Register<bool>
        //        {
        //            Address = i,
        //            Value = false,
        //            Name = "",
        //            RegisterType = RegisterType.DiscreteInput
        //        });
        //        Coils[i - 1] = (new Register<bool>
        //        {
        //            Address = i,
        //            Value = false,
        //            Name = "",
        //            RegisterType = RegisterType.Coil
        //        });
        //        DiscreteInputs[i - 1] = (new Register<short>
        //        {
        //            Address = i,
        //            Value = 0,
        //            Name = "",
        //            RegisterType = RegisterType.InputRegister
        //        });
        //        HoldingRegisters[i - 1] = (new Register<short>
        //        {
        //            Address = i,
        //            Value = 0,
        //            Name = "",
        //            RegisterType = RegisterType.HoldingRegister
        //        });
        //    }

        //    // Fill with saved configuration
        //    if (SelectedServer.WorkingConfiguration.RegisterModels.Count > 0)
        //    {

        //        for (int j = 0; j < SelectedServer.WorkingConfiguration.RegisterModels.Count; j++)
        //        {
        //            var reg = SelectedServer.WorkingConfiguration.RegisterModels[j];
        //            var i = reg.Address;
        //            var type = reg.Type;
        //            switch (type)
        //            {
        //                case RegisterType.DiscreteInput:
        //                    DiscreteInputs[i - 1] = (new Register<bool>
        //                    {
        //                        Address = i,
        //                        Value = SelectedServer.ReadDiscreteInput(i),
        //                        Name = reg.Name,
        //                        RegisterType = RegisterType.DiscreteInput
        //                    });
        //                    break;
        //                case RegisterType.Coil:
        //                    Coils[i - 1] = (new Register<bool>
        //                    {
        //                        Address = i,
        //                        Value = SelectedServer.ReadCoil(i),
        //                        Name = reg.Name,
        //                        RegisterType = RegisterType.Coil
        //                    });
        //                    break;
        //                case RegisterType.InputRegister:
        //                    DiscreteInputs[i - 1] = (new Register<short>
        //                    {
        //                        Address = i,
        //                        Value = SelectedServer.ReadInputRegister(i),
        //                        Name = reg.Name,
        //                        RegisterType = RegisterType.InputRegister
        //                    });
        //                    break;
        //                case RegisterType.HoldingRegister:
        //                    HoldingRegisters[i - 1] = (new Register<short>
        //                    {
        //                        Address = i,
        //                        Value = SelectedServer.ReadHoldingRegister(i),
        //                        Name = reg.Name,
        //                        RegisterType = RegisterType.HoldingRegister
        //                    });
        //                    break;
        //            }

        //        }
        //    } else
        //    {

        //        Trace.WriteLine("No registers"); // DELETE
        //    }

        //    // If you need to set CurrentItems to a default collection after update
        //    CurrentItems = HoldingRegisters;
        //}

        //public void SwitchTab(string tabName)
        //{
        //    CurrentTabName = tabName; // Keep track of the current tab
        //    if (onlyConfigured)
        //    {
        //        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //    }
            
        //    switch (tabName)
        //    {
        //        case "DiscreteInputs":
        //            CurrentItems = DiscreteInputs;
        //            if (onlyConfigured)
        //            {
        //                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //            }
        //            break;
        //        case "Coils":
        //            CurrentItems = Coils;
        //            if (onlyConfigured)
        //            {
        //                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //            }
        //            break;
        //        case "InputRegisters":
        //            CurrentItems = InputRegisters;
        //            if (onlyConfigured)
        //            {
        //                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //            }
        //            break;
        //        case "HoldingRegisters":
        //            CurrentItems = HoldingRegisters;
        //            if (onlyConfigured)
        //            {
        //                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //            }
        //            break;
        //    }
        //}

        ////variable to track if only configured registers should be shown during tab switches
        //bool onlyConfigured = false;
        //public void FilterModifiedRegisters(bool showOnlyModified)
        //{
        //    onlyConfigured = showOnlyModified;
        //    if (showOnlyModified)
        //    {
        //        // Filter the current tab's collection to only show modified items
        //        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
        //        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
        //    }
        //    else
        //    {
        //        // Reset CurrentItems to show all items for the current tab
        //        SwitchTab(CurrentTabName);
        //    }
        //}




        //public interface IRegister : INotifyPropertyChanged
        //{
        //    int Address { get; }
        //    object Value { get; set; }
        //    string Name { get; set; }
        //    RegisterType RegisterType { get; }
        //    bool IsModified { get; }
        //}

        //public class Register<T> : IRegister
        //{
        //    public required int Address { get; set; }
        //    public required T Value { get; set; }
        //    public required string Name { get; set; }
        //    public required RegisterType RegisterType { get; set; }

        //    public bool isModified = false;

        //    object IRegister.Value
        //    {
        //        get => Value;
        //        set
        //        {
        //            if (value is T)
        //            {
        //                this.Value = (T) value;
        //                IsModified = true;
        //                OnPropertyChanged(nameof(Value));
        //            } else
        //            {
        //                throw new InvalidOperationException($"Cannot assign value of type {value.GetType()} to type {typeof(T)}.");
        //            }
        //        }
        //    }

        //    string IRegister.Name
        //    {
        //        get => Name;
        //        set
        //        {
        //            if (Name != value)
        //            {
        //                Name = value;
        //                IsModified = true;
        //                OnPropertyChanged();
        //            }
        //        }
        //    }

        //    public bool IsModified
        //    {
        //        get => isModified;
        //        private set
        //        {
        //            if (isModified != value)
        //            {
        //                isModified = value;
        //                OnPropertyChanged(nameof(IsModified));
        //            }
        //        }
        //    }

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

    }
}
