using EasyModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static EasyModbus.ModbusServer;

namespace TestEase.Models
{
    public class ModbusServerModel : INotifyPropertyChanged
    {
        public int Port { get; set; }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged();
                }
            }
        }

        public ModbusServer Server { get; set; }

        public ConfigurationModel _workingConfiguration = new ConfigurationModel();
        public ConfigurationModel WorkingConfiguration
        {
            get => _workingConfiguration;
            set
            {
                if (_workingConfiguration != value)
                {
                    _workingConfiguration = value;
                    OnPropertyChanged(nameof(WorkingConfiguration));
                }
            }
        }

        public bool _isNotSaved = true;

        public bool IsNotSaved
        {
            get => _isNotSaved;
            set
            {
                if (_isNotSaved != value)
                {
                    _isNotSaved = value;
                    OnPropertyChanged(nameof(IsNotSaved));
                }
            }
        }

        // public ConfigurationModel WorkingConfiguration { get; set; } = new ConfigurationModel();

        public ModbusServerModel(int port)
        {
            this.Port = port;
            this.Server = new ModbusServer() { Port = port };

            for (int i = 1; i < 65535; i++)
            {
                this.DiscreteInputs.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.DiscreteInput
                });
                this.Coils.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.Coil
                });
                this.InputRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.InputRegister
                });
                this.HoldingRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.HoldingRegister
                });

                // Holding Registers by default
            }
            this._currentItems = HoldingRegisters;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<IRegister> DiscreteInputs { get; set; } = [];
        public ObservableCollection<IRegister> Coils { get; set; } = [];
        public ObservableCollection<IRegister> InputRegisters { get; set; } = [];
        public ObservableCollection<IRegister> HoldingRegisters { get; set; } = [];

        private ObservableCollection<IRegister>? _currentItems;
        public ObservableCollection<IRegister> CurrentItems
        {
            get => _currentItems;
            set
            {
                _currentItems = value;
                OnPropertyChanged(nameof(CurrentItems));
            }
        }

        private IRegister? _selectedRegister;
        public IRegister SelectedRegister
        {
            get => _selectedRegister;
            set
            {
                if (_selectedRegister != value)
                {
                    _selectedRegister = value;
                    OnPropertyChanged(nameof(SelectedRegister));
                    // Update IsRegisterSelected whenever SelectedRegister changes
                    IsRegisterSelected = _selectedRegister?.RegisterType == RegisterType.HoldingRegister ||
                                                _selectedRegister?.RegisterType == RegisterType.InputRegister;

                    // Update IsBooleanRegisterSelected based on the RegisterType
                    IsBooleanRegisterSelected = _selectedRegister?.RegisterType == RegisterType.DiscreteInput ||
                                                _selectedRegister?.RegisterType == RegisterType.Coil;
                }
            }
        }

        // Add IsRegisterSelected property to indicate if a register is selected
        private bool _isRegisterSelected;
        public bool IsRegisterSelected
        {
            get => _isRegisterSelected;
            set
            {
                _isRegisterSelected = value;
                OnPropertyChanged(nameof(IsRegisterSelected));
            }
            // set => _isRegisterSelected = value;
        }

        // Add IsBooleanRegisterSelected to indicate if a register is of discrete input or coil type
        private bool _isBooleanRegisterSelected;
        public bool IsBooleanRegisterSelected
        {
            get => _isBooleanRegisterSelected;
            set
            {
                _isBooleanRegisterSelected = value;
                OnPropertyChanged(nameof(IsBooleanRegisterSelected));
            }
            // set => _isBooleanRegisterSelected = value;
        }

        private bool _isCurveSelected;
        public bool IsCurveSelected
        {
            get { return _isCurveSelected; }
            set
            {
                if (_isCurveSelected != value)
                {
                    _isCurveSelected = value;
                    OnPropertyChanged(nameof(IsCurveSelected));
                }
            }
        }

        private bool _isRandomSelected;
        public bool IsRandomSelected
        {
            get { return _isRandomSelected; }
            set
            {
                if (_isRandomSelected != value)
                {
                    _isRandomSelected = value;
                    OnPropertyChanged(nameof(IsRandomSelected));
                }
            }
        }

        private bool _selectedBooleanValue;
        public bool SelectedBooleanValue
        {
            get => _selectedBooleanValue;
            set
            {
                if (_selectedBooleanValue != value)
                {
                    _selectedBooleanValue = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _isFloatConfigurationChecked;
        public bool IsFloatConfigurationChecked
        {
            get => _isFloatConfigurationChecked;
            set
            {
                if (_isFloatConfigurationChecked != value)
                {
                    _isFloatConfigurationChecked = value;
                    OnPropertyChanged(nameof(IsFloatConfigurationChecked));

                }
            }
        }

        private string _currentTabName = "HoldingRegisters"; // Default tab
        public string CurrentTabName
        {
            get => _currentTabName;
            set
            {
                if (_currentTabName != value)
                {
                    _currentTabName = value;
                    OnPropertyChanged(nameof(CurrentTabName));
                }
            }
        }

        private IEnumerable<IRegister> GetCurrentTabCollection()
        {
            return CurrentTabName switch
            {
                "DiscreteInputs" => DiscreteInputs,
                "Coils" => Coils,
                "InputRegisters" => InputRegisters,
                "HoldingRegisters" => HoldingRegisters,
                _ => Enumerable.Empty<IRegister>(),
            };
        }

        public void UpdateRegisterCollections()
        {

            CurrentItems = null;

            for (int i = 1; i < 65535; i++)
            {
                DiscreteInputs[i - 1] = (new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.DiscreteInput
                });
                Coils[i - 1] = (new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.Coil
                });
                InputRegisters[i - 1] = (new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.InputRegister
                });
                HoldingRegisters[i - 1] = (new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.HoldingRegister
                });
            }

            // Fill with saved configuration
            if (WorkingConfiguration.RegisterModels.Count > 0)
            {

                for (int j = 0; j < WorkingConfiguration.RegisterModels.Count; j++)
                {
                    var reg = WorkingConfiguration.RegisterModels[j];
                    var i = reg.Address;
                    var type = reg.Type;
                    switch (type)
                    {
                        case RegisterType.DiscreteInput:
                            DiscreteInputs[i - 1] = (new Register<bool>
                            {
                                Address = i,
                                Value = ReadDiscreteInput(i),
                                Name = reg.Name,
                                IsModified = true,
                                IsPlaying = reg.IsPlaying,
                                RegisterType = RegisterType.DiscreteInput
                            }); ;
                            break;
                        case RegisterType.Coil:
                            Coils[i - 1] = (new Register<bool>
                            {
                                Address = i,
                                Value = ReadCoil(i),
                                Name = reg.Name,
                                IsModified = true,
                                IsPlaying = reg.IsPlaying,
                                RegisterType = RegisterType.Coil
                            });
                            break;
                        case RegisterType.InputRegister:
                            InputRegisters[i - 1] = (new Register<short>
                            {
                                Address = i,
                                Value = ReadInputRegister(i),
                                Name = reg.Name,
                                IsModified = true,
                                IsPlaying = reg.IsPlaying,
                                RegisterType = RegisterType.InputRegister
                            });
                            break;
                        case RegisterType.HoldingRegister:
                            HoldingRegisters[i - 1] = (new Register<short>
                            {
                                Address = i,
                                Value = ReadHoldingRegister(i),
                                Name = reg.Name,
                                IsModified = true,
                                IsPlaying = reg.IsPlaying,
                                RegisterType = RegisterType.HoldingRegister
                            });
                            break;
                    }

                }
            }
            else
            {

                Trace.WriteLine("No registers"); // DELETE
            }

            // If you need to set CurrentItems to a default collection after update
            // CurrentItems = HoldingRegisters;
            // SwitchTab("HoldingRegisters");
            SwitchTab(CurrentTabName);

        }

        public void clearServerRegisters()
        {
            for (int j = 0; j < WorkingConfiguration.RegisterModels.Count; j++)
            {
                var reg = WorkingConfiguration.RegisterModels[j];
                var i = reg.Address;
                var type = reg.Type;
                switch (type)
                {
                    case RegisterType.DiscreteInput:
                        Server.discreteInputs[i] = false;
                        break;
                    case RegisterType.Coil:
                        Server.coils[i] = false;
                        break;
                    case RegisterType.InputRegister:
                        Server.inputRegisters[i] = 0;
                        if (reg is Fixed<float> || reg is Random<float> || reg is Curve<float>)
                        {
                            Server.inputRegisters[i + 1] = 0;
                        }
                        break;
                    case RegisterType.HoldingRegister:
                        Server.holdingRegisters[i] = 0;
                        if (reg is Fixed<float> || reg is Random<float> || reg is Curve<float>)
                        {
                            Server.holdingRegisters[i + 1] = 0;
                        }
                        break;
                }

            }
        }

        public void SwitchTab(string tabName)
        {
            CurrentTabName = tabName; // Keep track of the current tab
            if (onlyConfigured)
            {
                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
            }

            switch (tabName)
            {
                case "DiscreteInputs":
                    CurrentItems = DiscreteInputs;
                    if (onlyConfigured)
                    {
                        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
                    }
                    break;
                case "Coils":
                    CurrentItems = Coils;
                    if (onlyConfigured)
                    {
                        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
                    }
                    break;
                case "InputRegisters":
                    CurrentItems = InputRegisters;
                    if (onlyConfigured)
                    {
                        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
                    }
                    break;
                case "HoldingRegisters":
                    CurrentItems = HoldingRegisters;
                    if (onlyConfigured)
                    {
                        var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                        CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
                    }
                    break;
            }
        }

        //variable to track if only configured registers should be shown during tab switches
        bool onlyConfigured = false;
        public void FilterModifiedRegisters(bool showOnlyModified)
        {
            onlyConfigured = showOnlyModified;
            if (showOnlyModified)
            {
                // Filter the current tab's collection to only show modified items
                var modifiedItems = GetCurrentTabCollection().Where(item => item.IsModified).ToList();
                CurrentItems = new ObservableCollection<IRegister>(modifiedItems);
            }
            else
            {
                // Reset CurrentItems to show all items for the current tab
                SwitchTab(CurrentTabName);
            }
        }

        public void ResetRegistersToDefault()
        {
            // Reset DiscreteInputs and Coils to false
            foreach (var discreteInput in DiscreteInputs)
            {
                discreteInput.Value = false;
                discreteInput.Name = "";
                discreteInput.IsModified = false; // Assuming you want to reset modification status as well
                discreteInput.IsPlaying = false;
            }

            foreach (var coil in Coils)
            {
                coil.Value = false;
                coil.Name = "";
                coil.IsModified = false;
                coil.IsPlaying = false;
            }

            // Reset InputRegisters and HoldingRegisters to 0
            foreach (var inputRegister in InputRegisters)
            {
                inputRegister.Value = (short) 0;
                inputRegister.Name = "";
                inputRegister.IsModified = false;
                inputRegister.IsPlaying = false;
            }

            foreach (var holdingRegister in HoldingRegisters)
            {
                holdingRegister.Value = (short) 0;
                holdingRegister.Name = "";
                holdingRegister.IsModified = false;
                holdingRegister.IsPlaying = false;
            }

            // Notify the UI if necessary
            OnPropertyChanged(nameof(DiscreteInputs));
            OnPropertyChanged(nameof(Coils));
            OnPropertyChanged(nameof(InputRegisters));
            OnPropertyChanged(nameof(HoldingRegisters));
        }


        public interface IRegister : INotifyPropertyChanged
        {
            int Address { get; }
            object Value { get; set; }
            string Name { get; set; }
            RegisterType RegisterType { get; }
            bool IsModified { get; set; }
            bool IsFloatHelper { get; set; }
            bool IsPlaying { get; set; }
        }

        public class Register<T> : IRegister
        {
            public required int Address { get; set; }
            public required T Value { get; set; }
            public required string Name { get; set; }
            public required RegisterType RegisterType { get; set; }

            object IRegister.Value
            {
                get => Value;
                set
                {
                    if (value is T)
                    {
                        this.Value = (T)value;
                        IsModified = true;
                        IsPlaying = true;
                        OnPropertyChanged(nameof(Value));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot assign value of type {value.GetType()} to type {typeof(T)}.");
                    }
                }
            }

            string IRegister.Name
            {
                get => Name;
                set
                {
                    if (Name != value)
                    {
                        Name = value;
                        IsModified = true;
                        OnPropertyChanged();
                    }
                }
            }

            private bool _isModified = false;
            public bool IsModified
            {
                get => _isModified;
                set
                {
                    if (_isModified != value)
                    {
                        _isModified = value;
                        OnPropertyChanged(nameof(IsModified));
                    }
                }
            }

            private bool _isFloatHelper = false;
            public bool IsFloatHelper
            {
                get => _isFloatHelper;
                set
                {
                    if (_isFloatHelper != value)
                    {
                        _isFloatHelper = value;
                        OnPropertyChanged(nameof(IsFloatHelper));
                    }
                }
            }

            private bool _IsPlaying = false;
            public bool IsPlaying
            {
                get => _IsPlaying;
                set
                {
                    if (_IsPlaying != value)
                    {
                        _IsPlaying = value;
                        OnPropertyChanged(nameof(IsPlaying));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public void StartServer()
        {
            Server.Listen();
        }

        public void StopServer()
        {
            Server.StopListening();
        }

        // HOLDING REGISTERS
        public short ReadHoldingRegister(int address)
        {
            return Server.holdingRegisters[address];
        }
        public void WriteHoldingRegister(int address, short value)
        {
            Server.holdingRegisters[address] = value;
        }

        public short[] GetHoldingRegisters()
        {
            return Server.holdingRegisters.localArray;
        }

        // INPUT REGISTERS
        public short ReadInputRegister(int address)
        {
            return Server.inputRegisters[address];
        }
        public void WriteInputRegister(int address, short value)
        {
            Server.inputRegisters[address] = value;
        }

        public short[] GetInputRegisters()
        {
            return Server.inputRegisters.localArray;
        }

        // DISCRETE INTPUTS
        public bool ReadDiscreteInput(int address)
        {
            return Server.discreteInputs[address];
        }
        public void WriteDiscreteInput(int address, bool value)
        {
            Server.discreteInputs[address] = value;
        }

        public bool[] GetDiscreteInputs()
        {
            return Server.discreteInputs.localArray;
        }

        // COILS
        public bool ReadCoil(int address)
        {
            return Server.coils[address];
        }
        public void WriteCoil(int address, bool value)
        {
            Server.coils[address] = value;
        }

        public bool[] GetCoils()
        {
            return Server.coils.localArray;
        }


    }
}
