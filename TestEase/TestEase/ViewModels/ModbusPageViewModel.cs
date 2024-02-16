using CommunityToolkit.Mvvm.ComponentModel;
using EasyModbus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEase.Models;
using TestEase.Services;
using static EasyModbus.ModbusServer;

namespace TestEase.ViewModels
{
    public partial class ModbusPageViewModel: ObservableObject
    {

        public ModbusServerModel SelectedServer { get; set; }

        public ObservableCollection<Register> HoldingItems { get; } = new();

        private ModbusService service;
        public ModbusPageViewModel()
        {
            service = new ModbusService();
            SelectedServer = new ModbusServerModel(502);
            for (int i = 1; i < 60001; i++)
            {
                HoldingItems.Add(new Register
                {
                    Address = i,
                    Value = 0,
                    Name = ""
                });
                
            }
            
            /*
            service.CreateServer(502);
            service.StartServer(502);
            short[] holdingRegisters = service.GetHoldingRegisters(502);
            if (holdingRegisters != null)
            {
                for (int i = 0; i < holdingRegisters.Length; i++)
                {
                    HoldingItems.Add(new Register
                    {
                        Id = i,
                        Value = holdingRegisters[i],
                        Name = ""
                    });
                }
            } else
            {
                Console.WriteLine("Holding Registers is null");
            }
            service.StopServer(502);
            */

        }

        public class Register
        {
            public int Address { get; set; }
            public short Value { get; set; }
            public string Name { get; set; } = "";

        }



    }
}
