using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private ModbusService service;
        public ModbusPageViewModel()
        {
            service = new ModbusService();
            SelectedServer = new ModbusServerModel(502);
            for (int i = 1; i < 65535; i++)
            {
                DiscreteInputs.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "discrete"
                });
                Coils.Add(new Register<bool>
                {
                    Address = i,
                    Value = false,
                    Name = "coils"
                });
                InputRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "input"
                });
                HoldingRegisters.Add(new Register<short>
                {
                    Address = i,
                    Value = 0,
                    Name = "holding"
                });

                // Holding Registers by default
                CurrentItems = HoldingRegisters;
                
            }
            
            /*
            service.CreateServer(502);
            service.StartServer(502);
            short[] holdingRegisters = service.GetHoldingRegisters(502);
            if (holdingRegisters != null)
            {
                for (int i = 0; i < holdingRegisters.Length; i++)
                {
                    HoldingItems.Add(new Register
                    {
                        Id = i,
                        Value = holdingRegisters[i],
                        Name = ""
                    });
                }
            } else
            {
                Console.WriteLine("Holding Registers is null");
            }
            service.StopServer(502);
            */

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

        public interface IRegister
        {
            int Address { get; }
            string Name { get; }
            object Value { get; }
        }

        public class Register<T> : IRegister
        {
            public required int Address { get; set; }
            public required T Value { get; set; }
            public required String Name { get; set; }

            object IRegister.Value => Value;
        }

    }
}
