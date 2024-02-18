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


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
