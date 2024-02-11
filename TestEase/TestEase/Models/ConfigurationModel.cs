using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Models
{
    public class ConfigurationModel
    {
        public List<RegisterModel> RegisterModels { get; set; }

        public string Name { get; set; }
        
        public ConfigurationModel()
        {
            RegisterModels = [];
            Name = "new config";
        }

        public ConfigurationModel(String name)
        {
            RegisterModels = [];
            Name = name;
        }
    }
}
