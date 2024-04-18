using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEase.Models;
using System.Text.Json;

namespace TestEase.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {

        // Event declaration required by INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to invoke the PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Backing fields for properties
        private ObservableCollection<ModbusServerModel> _modbusServers = new ObservableCollection<ModbusServerModel>();
        private ObservableCollection<ConfigurationModel> _configurations = new ObservableCollection<ConfigurationModel>();

        // Properties
        public ObservableCollection<ModbusServerModel> ModbusServers
        {
            get => _modbusServers;
            set
            {
                if (_modbusServers != value)
                {
                    _modbusServers = value;
                    OnPropertyChanged(nameof(ModbusServers));
                }
            }
        }

        // public ObservableCollection<ConfigurationModel> Configurations { get; set; } = new ObservableCollection<ConfigurationModel>();
        public ObservableCollection<ConfigurationModel> Configurations
        {
            get => _configurations;
            set
            {
                if (_configurations != value)
                {
                    _configurations = value;
                    OnPropertyChanged(nameof(Configurations));
                }
            }
        }

        public void SaveServers(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new ModbusServerModelConverter());

            string jsonString = JsonSerializer.Serialize(ModbusServers, options);
            File.WriteAllText(filePath, jsonString);
        }

        public void LoadServers(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions();
                options.Converters.Add(new ModbusServerModelConverter());
                var servers = JsonSerializer.Deserialize<ObservableCollection<ModbusServerModel>>(jsonString, options);
                if (servers != null)
                {
                    ModbusServers.Clear();
                    foreach (var server in servers)
                    {
                        ModbusServers.Add(server);
                    }
                }
            }
        }
    }
}
