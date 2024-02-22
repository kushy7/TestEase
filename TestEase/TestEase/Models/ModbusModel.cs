using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Models
{
    public class ModbusModel
    {
        public ModbusServerModel SelectedServer { get; set; }

        public ModbusModel()
        {
            SelectedServer = new ModbusServerModel(502);
        }
    }
}
