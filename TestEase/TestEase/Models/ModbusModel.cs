using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing3D;

namespace TestEase.Models
{
    public class ModbusModel
    {
        public int Port { get; set; }
        public bool IsRunning { get; set; }

        public List<RegisterModel> Registers { get; set; }
        public ModbusModel(int port)
        {
            Port = port;
            Registers = new List<RegisterModel>();
        }
    }
}
