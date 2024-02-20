using EasyModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Models
{
    public class ModbusServerModel(int port) : INotifyPropertyChanged
    {
        public int Port { get; set; } = port;

        private bool _isRunning = false;

        public ModbusServer Server { get; set; } = new ModbusServer()
        {
            Port = port
        };

        public ConfigurationModel WorkingConfiguration { get; set; } = new ConfigurationModel();

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


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
