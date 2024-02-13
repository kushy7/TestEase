using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Models
{
    public class ModbusServerModel(int port)
    {
        public int Port { get; set; } = port;
        public bool IsRunning { get; set; }

        public ConfigurationModel WorkingConfiguration { get; set; } = new ConfigurationModel();
    }
}
