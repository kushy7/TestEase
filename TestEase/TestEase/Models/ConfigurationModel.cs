using System;
using System.Collections.Generic;

namespace TestEase.Models
{
    public class ConfigurationModel
    {
        public List<RegisterModel> RegisterModels { get; set; }

        public string Name { get; set; }

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
    }
}
