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

        private int _iterationStep = 0;



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
                        if (register is Random<short> r)
                        {
                            var val = ValueGenerators.GenerateRandomValueShort(r.startValue, r.endValue);
                            server.WriteHoldingRegister(register.Address, val);
                            server.HoldingRegisters[register.Address - 1].Value = val;
                        }
                        else if (register is Random<float> rf)
                        {
                            float randomValue = ValueGenerators.GenerateRandomValueFloat(rf.startValue, rf.endValue);
                            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(randomValue);
                            short lowBits = lowHighBits[0];
                            short highBits = lowHighBits[1];
                            
                            server.WriteHoldingRegister(register.Address, lowBits);
                            server.WriteHoldingRegister(register.Address + 1, highBits);
                            server.HoldingRegisters[register.Address - 1].Value = lowBits;
                            server.HoldingRegisters[register.Address].Value = highBits;
                        }
                        else if (register is Curve<short> ra)
                        {

                            ra.IncrementIterationStep(); // Increment iterationStep
                            var val = ValueGenerators.GenerateNextSinValue(ra.startValue, ra.endValue, ra.GetIterationStep(), ra.Period);
                            server.WriteHoldingRegister(register.Address, val);
                            server.HoldingRegisters[register.Address - 1].Value = val;
                        }
                        else if (register is Curve<float> raf)
                        {

                            raf.IncrementIterationStep(); // Increment iterationStep
                            float nextValue = ValueGenerators.GetNextSineValueFloat(raf.startValue, raf.endValue, raf.GetIterationStep(), raf.Period);
                            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                            short lowBits = lowHighBits[0];
                            short highBits = lowHighBits[1];

                            server.WriteHoldingRegister(register.Address, lowBits);
                            server.WriteHoldingRegister(register.Address + 1, highBits);
                            server.HoldingRegisters[register.Address - 1].Value = lowBits;
                            server.HoldingRegisters[register.Address].Value = highBits;
                        }
                    } else if (register.Type == RegisterType.InputRegister)
                    {
                        if(register is Random<short> r)
                        {
                            server.WriteInputRegister(register.Address, ValueGenerators.GenerateRandomValueShort(r.startValue, r.endValue));
                        } else if (register is Random<float> rf)
                        {
                            float randomValue = ValueGenerators.GenerateRandomValueFloat(rf.startValue, rf.endValue);
                            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(randomValue);
                            short lowBits = lowHighBits[0];
                            short highBits = lowHighBits[1];

                            server.WriteInputRegister(register.Address, lowBits);
                            server.WriteInputRegister(register.Address + 1, highBits);
                            server.InputRegisters[register.Address - 1].Value = lowBits;
                            server.InputRegisters[register.Address].Value = highBits;
                        }
                        else if (register is Curve<short> ra)
                        {

                            ra.IncrementIterationStep(); // Increment iterationStep
                            var val = ValueGenerators.GenerateNextSinValue(ra.startValue, ra.endValue, ra.GetIterationStep(), ra.Period);
                            server.WriteInputRegister(register.Address, val);
                            server.InputRegisters[register.Address - 1].Value = val;
                        }
                        else if (register is Curve<float> raf)
                        {

                            raf.IncrementIterationStep(); // Increment iterationStep
                            float nextValue = ValueGenerators.GetNextSineValueFloat(raf.startValue, raf.endValue, raf.GetIterationStep(), raf.Period);
                            short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                            short lowBits = lowHighBits[0];
                            short highBits = lowHighBits[1];

                            server.WriteInputRegister(register.Address, lowBits);
                            server.WriteInputRegister(register.Address + 1, highBits);
                            server.InputRegisters[register.Address - 1].Value = lowBits;
                            server.InputRegisters[register.Address].Value = highBits;
                        }
                    }
                    else if (register.Type == RegisterType.InputRegister)
                    {
                        if (register is Random<short> r)
                        {
                            var val = ValueGenerators.GenerateRandomValueShort(r.startValue, r.endValue);
                            server.WriteInputRegister(register.Address, val);
                            server.InputRegisters[register.Address - 1].Value = val;
                        }
                    }
                }
            }

            // If needed, raise an event or use a messaging system to notify the UI of updates
        }

       


    }
}
