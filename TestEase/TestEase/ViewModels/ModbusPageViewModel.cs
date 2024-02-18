using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestEase.Models;
using TestEase.Services;
using static EasyModbus.ModbusServer;

namespace TestEase.ViewModels
{
    public partial class ModbusPageViewModel: ObservableObject
    {

        public ModbusServerModel SelectedServer { get; set; }

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

        public ModbusService Service { get; }

        public AppViewModel AppViewModel { get; }
        public ModbusPageViewModel(AppViewModel appViewModel)
        {
            AppViewModel = appViewModel;
            Service = new ModbusService(appViewModel);

            Service.CreateServer(502);
            Service.StartServer(502);
            SelectedServer = AppViewModel.ModbusServers[0];
            SelectedServer.IsRunning = true;


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
