using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using EasyModbus;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.ViewModels;
//using Windows.Graphics.Printing3D;

namespace TestEase.Services
{
    public class ModbusService(AppViewModel appViewModel)
    {

        private Timer _updateTimer;

        public void StartPeriodicUpdate(TimeSpan interval)
        {
            _updateTimer = new Timer(UpdateRegistersCallback, null, TimeSpan.Zero, interval);
        }

        private void UpdateRegistersCallback(object state)
        {
            foreach (var server in appViewModel.ModbusServers)
            {
                // Logic to update each server's registers
                // This might involve generating new values and updating the Modbus registers
                foreach (var register in server.WorkingConfiguration.RegisterModels)
                {
                    if (register.Type == RegisterType.HoldingRegister)
                    {
                        // server.WriteHoldingRegister(register.Address, (short) (server.ReadHoldingRegister(register.Address) + 1));
                        if (register is Random<short> r)
                        {
                            server.WriteHoldingRegister(register.Address, ValueGenerators.GenerateRandomValueShort(r.startValue, r.endValue));
                        }
                    } else if (register.Type == RegisterType.InputRegister)
                    {
                        if(register is Random<short> r)
                        {
                            server.WriteInputRegister(register.Address, ValueGenerators.GenerateRandomValueShort(r.startValue, r.endValue));
                        }
                    }
                }
            }

            // If needed, raise an event or use a messaging system to notify the UI of updates
        }


    }
}
