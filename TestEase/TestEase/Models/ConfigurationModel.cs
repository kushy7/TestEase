using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestEase.Models
{
    //each configuration file has a list of the models and a name
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

        //defaults the name to new config
        public ConfigurationModel()
        {
            RegisterModels = new List<RegisterModel>();
            Name = "new config";
        }

        //this lets the user set the name
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

        //adds all the registers options/settings into the config
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
