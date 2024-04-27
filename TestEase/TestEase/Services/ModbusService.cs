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


//this is the service that updates the values for each register every 3 seconds
namespace TestEase.Services
{
    public class ModbusService(AppViewModel appViewModel)
    {

        private Timer _updateTimer;

        private int _iterationStep = 0;


        //updates the values depending on the "interval" passed into this function
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
                if (server.IsRunning)
                {
                    foreach (var register in server.WorkingConfiguration.RegisterModels)
                    {
                        if (register.IsPlaying == false)
                        {
                            continue;
                        }
                        // HOLDING REGISTERS
                        if (register.Type == RegisterType.HoldingRegister)
                        {
                            if (register is Fixed<short> fs)
                            {
                                server.WriteHoldingRegister(register.Address, fs.Value);
                                server.HoldingRegisters[register.Address - 1].Value = fs.Value;
                            } 
                            else if (register is Fixed<float> ff)
                            {
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(ff.Value);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteHoldingRegister(register.Address, lowBits);
                                server.WriteHoldingRegister(register.Address + 1, highBits);
                                server.HoldingRegisters[register.Address - 1].Value = lowBits;
                                server.HoldingRegisters[register.Address].Value = highBits;
                                server.HoldingRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Random<short> r)
                            {
                                var val = ValueGenerators.GenerateRandomValueShort(r.StartValue, r.EndValue);
                                server.WriteHoldingRegister(register.Address, val);
                                server.HoldingRegisters[register.Address - 1].Value = val;
                            }
                            else if (register is Random<float> rf)
                            {
                                float randomValue = ValueGenerators.GenerateRandomValueFloat(rf.StartValue, rf.EndValue);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(randomValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];
                            
                                server.WriteHoldingRegister(register.Address, lowBits);
                                server.WriteHoldingRegister(register.Address + 1, highBits);
                                server.HoldingRegisters[register.Address - 1].Value = lowBits;
                                server.HoldingRegisters[register.Address].Value = highBits;
                                server.HoldingRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Curve<short> ra)
                            {

                                ra.IncrementIterationStep(); // Increment iterationStep
                                var val = ValueGenerators.GenerateNextSinValue(ra.StartValue, ra.EndValue, ra.GetIterationStep(), ra.Period);
                                server.WriteHoldingRegister(register.Address, val);
                                server.HoldingRegisters[register.Address - 1].Value = val;
                            }
                            else if (register is Curve<float> raf)
                            {

                                raf.IncrementIterationStep(); // Increment iterationStep
                                float nextValue = ValueGenerators.GetNextSineValueFloat(raf.StartValue, raf.EndValue, raf.GetIterationStep(), raf.Period);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteHoldingRegister(register.Address, lowBits);
                                server.WriteHoldingRegister(register.Address + 1, highBits);
                                server.HoldingRegisters[register.Address - 1].Value = lowBits;
                                server.HoldingRegisters[register.Address].Value = highBits;
                                server.HoldingRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Linear<short> la)
                            {

                                short current = la.GetCurrentValue();
                                bool increasing = true;
                                var val = ValueGenerators.GenerateLinearValue(current, la.StartValue, la.EndValue, la.Increment, ref increasing);
                                server.WriteHoldingRegister(register.Address, val);
                                server.HoldingRegisters[register.Address - 1].Value = val;
                            }
                            else if (register is Linear<float> laf)
                            {

                                float current = laf.GetCurrentValue();
                                bool increasing = true;
                                float nextValue = ValueGenerators.GenerateLinearValueFloat(current, laf.StartValue, laf.EndValue, laf.Increment, ref increasing);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteHoldingRegister(register.Address, lowBits);
                                server.WriteHoldingRegister(register.Address + 1, highBits);
                                server.HoldingRegisters[register.Address - 1].Value = lowBits;
                                server.HoldingRegisters[register.Address].Value = highBits;
                                server.HoldingRegisters[register.Address].IsFloatHelper = true;
                            }
                        } 
                        // INPUT REGISTERS
                        else if (register.Type == RegisterType.InputRegister)
                        {
                            if (register is Fixed<short> fs)
                            {
                                server.WriteInputRegister(register.Address, fs.Value);
                                server.InputRegisters[register.Address - 1].Value = fs.Value;
                            }
                            else if (register is Fixed<float> ff)
                            {
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(ff.Value);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteInputRegister(register.Address, lowBits);
                                server.WriteInputRegister(register.Address + 1, highBits);
                                server.InputRegisters[register.Address - 1].Value = lowBits;
                                server.InputRegisters[register.Address].Value = highBits;
                                server.InputRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Random<short> r)
                            {
                                var val = ValueGenerators.GenerateRandomValueShort(r.StartValue, r.EndValue);
                                server.WriteInputRegister(register.Address, val);
                                server.InputRegisters[register.Address - 1].Value = val;
                            } 
                            else if (register is Random<float> rf)
                            {
                                float randomValue = ValueGenerators.GenerateRandomValueFloat(rf.StartValue, rf.EndValue);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(randomValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteInputRegister(register.Address, lowBits);
                                server.WriteInputRegister(register.Address + 1, highBits);
                                server.InputRegisters[register.Address - 1].Value = lowBits;
                                server.InputRegisters[register.Address].Value = highBits;
                                server.InputRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Curve<short> ra)
                            {

                                ra.IncrementIterationStep(); // Increment iterationStep
                                var val = ValueGenerators.GenerateNextSinValue(ra.StartValue, ra.EndValue, ra.GetIterationStep(), ra.Period);
                                server.WriteInputRegister(register.Address, val);
                                server.InputRegisters[register.Address - 1].Value = val;
                            }
                            else if (register is Curve<float> raf)
                            {

                                raf.IncrementIterationStep(); // Increment iterationStep
                                float nextValue = ValueGenerators.GetNextSineValueFloat(raf.StartValue, raf.EndValue, raf.GetIterationStep(), raf.Period);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteInputRegister(register.Address, lowBits);
                                server.WriteInputRegister(register.Address + 1, highBits);
                                server.InputRegisters[register.Address - 1].Value = lowBits;
                                server.InputRegisters[register.Address].Value = highBits;
                                server.InputRegisters[register.Address].IsFloatHelper = true;
                            }
                            else if (register is Linear<short> la)
                            {

                                short current = la.GetCurrentValue();
                                bool increasing = true;
                                var val = ValueGenerators.GenerateLinearValue(current, la.StartValue, la.EndValue, la.Increment, ref increasing);
                                server.WriteInputRegister(register.Address, val);
                                server.InputRegisters[register.Address - 1].Value = val;
                            }
                            else if (register is Linear<float> laf)
                            {

                                float current = laf.GetCurrentValue();
                                bool increasing = true;
                                float nextValue = ValueGenerators.GenerateLinearValueFloat(current, laf.StartValue, laf.EndValue, laf.Increment, ref increasing);
                                short[] lowHighBits = ValueGenerators.GenerateShortArrayFromFloat(nextValue);
                                short lowBits = lowHighBits[0];
                                short highBits = lowHighBits[1];

                                server.WriteInputRegister(register.Address, lowBits);
                                server.WriteInputRegister(register.Address + 1, highBits);
                                server.InputRegisters[register.Address - 1].Value = lowBits;
                                server.InputRegisters[register.Address].Value = highBits;
                                server.InputRegisters[register.Address].IsFloatHelper = true;
                            }
                        }
                        // DISCRETE INPUTS
                        else if (register.Type == RegisterType.DiscreteInput)
                        {
                            if (register is CoilOrDiscrete r)
                            {
                                server.WriteDiscreteInput(register.Address, r.Value);
                                server.DiscreteInputs[register.Address - 1].Value = r.Value;
                            }
                        }
                        // COILS
                        else if (register.Type == RegisterType.Coil)
                        {
                            if (register is CoilOrDiscrete r)
                            {
                                server.WriteCoil(register.Address, r.Value);
                                server.Coils[register.Address - 1].Value = r.Value;
                            }
                        }
                    }
                }
                
            }

            // If needed, raise an event or use a messaging system to notify the UI of updates
        }

       


    }
}
