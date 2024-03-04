using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestEase.Models
{
    public class ConfigurationModel : INotifyPropertyChanged
    {
        public List<RegisterModel> RegisterModels { get; set; }

        public string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ConfigurationModel()
        {
            RegisterModels = new List<RegisterModel>();
            Name = "new config";
        }

        public ConfigurationModel(string name)
        {
            RegisterModels = new List<RegisterModel>();
            Name = name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ConfigurationModel DeepCopy()
        {
            ConfigurationModel copy = new ConfigurationModel(Name);
            foreach( var model in RegisterModels)
            {
                copy.RegisterModels.Add(model);
            }
            return copy;
        }
    }
}
