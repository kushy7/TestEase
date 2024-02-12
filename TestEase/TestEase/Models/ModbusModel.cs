using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Graphics.Printing3D;

namespace TestEase.Models
{
    public class ModbusModel(int port)
    {
        public int Port { get; set; } = port;
        public bool IsRunning { get; set; }

        public ConfigurationModel WorkingConfiguration { get; set; } = new ConfigurationModel();
    }
}
