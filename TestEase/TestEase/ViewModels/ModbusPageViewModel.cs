using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                    if (CurrentItems != null)
                    {
                        UpdateRegisterCollections();
                    }
                    
                }
            }
        }

        public ObservableCollection<IRegister> DiscreteInputs { get; set; } = new();
        public ObservableCollection<IRegister> Coils { get; set; } = new();
        public ObservableCollection<IRegister> InputRegisters { get; set; } = new();
        public ObservableCollection<IRegister> HoldingRegisters { get; set; } = new();

        private ObservableCollection<IRegister> _currentItems;
        public ObservableCollection<IRegister> CurrentItems
        {
            get => _currentItems;
            set
            {
                _currentItems = value;
                OnPropertyChanged(nameof(CurrentItems));
            }
        }

        private IRegister _selectedRegister;
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
                    IsRegisterSelected = _selectedRegister != null;
                }
            }
        }

        // Add IsRegisterSelected property to indicate if a register is selected
        private bool _isRegisterSelected;
        public bool IsRegisterSelected
        {
            get => _isRegisterSelected;
            set => SetProperty(ref _isRegisterSelected, value);
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


            for (int i = 1; i < 65535; i++)
            {
                DiscreteInputs.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.DiscreteInput
                });
                Coils.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "",
                    RegisterType = RegisterType.Coil
                });
                InputRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.InputRegister
                });
                HoldingRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "",
                    RegisterType = RegisterType.HoldingRegister
                });

                // Holding Registers by default
                CurrentItems = HoldingRegisters;
                
            }

        }

        private void UpdateRegisterCollections()
        {
            // Assuming you want to clear the existing collections and repopulate them
            //DiscreteInputs.Clear();
            //Coils.Clear();
            //InputRegisters.Clear();
            //HoldingRegisters.Clear();
            Trace.WriteLine("Updating Register collections"); // DELETE

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
                DiscreteInputs[i - 1] = (new Register<short>
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
            if (SelectedServer.WorkingConfiguration.RegisterModels.Count > 0)
            {

                for (int j = 0; j < SelectedServer.WorkingConfiguration.RegisterModels.Count; j++)
                {
                    var reg = SelectedServer.WorkingConfiguration.RegisterModels[j];
                    var i = reg.Address;
                    var type = reg.Type;
                    switch (type)
                    {
                        case RegisterType.DiscreteInput:
                            DiscreteInputs[i - 1] = (new Register<bool>
                            {
                                Address = i,
                                Value = SelectedServer.ReadDiscreteInput(i),
                                Name = reg.Name,
                                RegisterType = RegisterType.DiscreteInput
                            });
                            break;
                        case RegisterType.Coil:
                            Coils[i - 1] = (new Register<bool>
                            {
                                Address = i,
                                Value = SelectedServer.ReadCoil(i),
                                Name = reg.Name,
                                RegisterType = RegisterType.Coil
                            });
                            break;
                        case RegisterType.InputRegister:
                            DiscreteInputs[i - 1] = (new Register<short>
                            {
                                Address = i,
                                Value = SelectedServer.ReadInputRegister(i),
                                Name = reg.Name,
                                RegisterType = RegisterType.InputRegister
                            });
                            break;
                        case RegisterType.HoldingRegister:
                            HoldingRegisters[i - 1] = (new Register<short>
                            {
                                Address = i,
                                Value = SelectedServer.ReadHoldingRegister(i),
                                Name = reg.Name,
                                RegisterType = RegisterType.HoldingRegister
                            });
                            break;
                    }

                }
            } else
            {

                Trace.WriteLine("No registers"); // DELETE
            }

            // If you need to set CurrentItems to a default collection after update
            CurrentItems = HoldingRegisters;
        }

        public void SwitchTab(string tabName)
        {
            switch (tabName)
            {
                case "DiscreteInputs":
                    CurrentItems = DiscreteInputs;
                    break;
                case "Coils":
                    CurrentItems = Coils;
                    break;
                case "InputRegisters":
                    CurrentItems = InputRegisters;
                    break;
                case "HoldingRegisters":
                    CurrentItems = HoldingRegisters;
                    break;
            }
        }

        public interface IRegister : INotifyPropertyChanged
        {
            int Address { get; }
            object Value { get; set; }
            string Name { get; set; }
            RegisterType RegisterType { get; }
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
                        this.Value = (T) value;
                        OnPropertyChanged(nameof(Value));
                    } else
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
                        OnPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
