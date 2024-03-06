using EasyModbus;
using System.Runtime.CompilerServices;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.Services;
using Xunit.Sdk;
using Moq;
using System.ComponentModel;
using static TestEase.Models.ModbusServerModel;
using System.Collections.ObjectModel;

namespace TestEaseTest
{
    public class ModbusServerTest
    {
        [Fact]
        public void testReadWriteHoldingRegister()
        {
            var serverModel = new ModbusServerModel(502);
            serverModel.WriteHoldingRegister(1, 1);
            short reg = serverModel.ReadHoldingRegister(1);
            Assert.IsType<short>(reg);
            Assert.Equal(1, reg);
            short[] regArray = serverModel.GetHoldingRegisters();
            Assert.Equal(1, regArray[1]);
        }

        [Fact]
        public void testReadWriteInputRegister()
        {
            var serverModel = new ModbusServerModel(502);
            serverModel.WriteInputRegister(1, 1);
            short reg = serverModel.ReadInputRegister(1);
            Assert.IsType<short>(reg);
            Assert.Equal(1, reg);
            short[] regArray = serverModel.GetInputRegisters();
            Assert.Equal(1, regArray[1]);
        }

        [Fact]
        public void testReadWriteDiscreteInput()
        {

            var serverModel = new ModbusServerModel(502);
            serverModel.StartServer();
            serverModel.WriteDiscreteInput(1, true);
            bool reg = serverModel.ReadDiscreteInput(1);
            Assert.IsType<bool>(reg);
            Assert.True(reg);
            bool[] regArray = serverModel.GetDiscreteInputs();
            Assert.True(regArray[1]);
            serverModel.StopServer();
        }

        [Fact]
        public void testReadWriteCoil()
        {

            var serverModel = new ModbusServerModel(502);
            serverModel.WriteCoil(1, true);
            bool reg = serverModel.ReadCoil(1);
            Assert.IsType<bool>(reg);
            Assert.True(reg);
            bool[] regArray = serverModel.GetCoils();
            Assert.True(regArray[1]);
        }

        [Fact]
        public void testModbusModel()
        {
            ModbusModel temp = new ModbusModel();
            Assert.NotNull(temp);
        }

        [Fact]
        public void testIsRunning()
        {
            var serverModel = new ModbusServerModel(502);
            var mockPropertyChanged = new Mock<INotifyPropertyChanged>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.IsRunning = true;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testIsNotSaved()
        {
            var serverModel = new ModbusServerModel(502);
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.IsNotSaved = false;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testCurrentTimes()
        {
            var serverModel = new ModbusServerModel(502);
            var items = new ObservableCollection<IRegister>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.CurrentItems = items;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testIsCurveSelected()
        {
            var serverModel = new ModbusServerModel(502);
            var items = new ObservableCollection<IRegister>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.IsCurveSelected = true;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testIsRandomSelected()
        {
            var serverModel = new ModbusServerModel(502);
            var items = new ObservableCollection<IRegister>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.IsRandomSelected = true;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testIsFloatSelected()
        {
            var serverModel = new ModbusServerModel(502);
            var items = new ObservableCollection<IRegister>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.IsFloatConfigurationChecked = true;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testIsBooleanSelected()
        {
            var serverModel = new ModbusServerModel(502);
            var items = new ObservableCollection<IRegister>();
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.SelectedBooleanValue = true;

            Assert.True(eventRaised);
        }

        [Fact]
        public void testSelectedRegister()
        {
            var serverModel = new ModbusServerModel(502);

            var coilRegister = new Register<bool>
            {
                Address = 1,
                Value = true,
                Name = "test",
                RegisterType = RegisterType.Coil
            };

            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.SelectedRegister = coilRegister;

            Assert.True(eventRaised);
        }



        [Fact]
        public void testModbusServerModelWorkingConfiguration()
        {
            var serverModel = new ModbusServerModel(502);
            serverModel.ResetRegistersToDefault();
            var newConfig = new ConfigurationModel("new");
            bool eventRaised = false;
            serverModel.PropertyChanged += (sender, args) => { eventRaised = true; };

            serverModel.WorkingConfiguration = newConfig;

            Assert.True(eventRaised);
            Assert.Equal(newConfig, serverModel.WorkingConfiguration);
        }

        [Fact]
        public void testUpdateRegisterCollections()
        {
            var serverModel = new ModbusServerModel(502);
            var mockRegisterModels = new List<RegisterModel>
            {
                new CoilOrDiscrete(1, RegisterType.DiscreteInput, "DiscreteInput", true),
                new CoilOrDiscrete(1, RegisterType.Coil, "CoilInput", true),
                new Fixed<int>(1, RegisterType.HoldingRegister, "HoldingRegister", 2, false),
                new Fixed<int>(1, RegisterType.InputRegister, "InputRegister", 2, false),
                new Fixed<float>(1, RegisterType.HoldingRegister, "HoldingFloat", 2.0f, true),
                new Fixed<float>(1, RegisterType.InputRegister, "InputRegister", 2.0f, true),



            };
            serverModel.WorkingConfiguration.RegisterModels = mockRegisterModels;
            serverModel.UpdateRegisterCollections();
            serverModel.clearServerRegisters();
            Assert.Equal(65534, serverModel.DiscreteInputs.Count);
            Assert.Equal(65534, serverModel.Coils.Count);
            Assert.Equal(65534, serverModel.InputRegisters.Count);
            Assert.Equal(65534, serverModel.HoldingRegisters.Count);
        }

        [Fact]
        public void testSwitchTabs()
        {
            var serverModel = new ModbusServerModel(502);
            serverModel.Coils = new ObservableCollection<IRegister>
            {
                new Register<bool> { Address = 1, Value = true, Name = "Test1", RegisterType = RegisterType.Coil },
                new Register<bool> { Address = 2, Value = false, Name = "Test2", RegisterType = RegisterType.Coil }
            };

            serverModel.SwitchTab("Coils");

            Assert.Equal(serverModel.Coils, serverModel.CurrentItems);

            serverModel = new ModbusServerModel(502);
            serverModel.DiscreteInputs = new ObservableCollection<IRegister>
            {
                new Register<bool> { Address = 1, Value = true, Name = "Test1", RegisterType = RegisterType.DiscreteInput },
                new Register<bool> { Address = 2, Value = false, Name = "Test2", RegisterType = RegisterType.DiscreteInput }
            };

            serverModel.SwitchTab("DiscreteInputs");

            Assert.Equal(serverModel.DiscreteInputs, serverModel.CurrentItems);

            serverModel = new ModbusServerModel(502);
            serverModel.HoldingRegisters = new ObservableCollection<IRegister>
            {
                new Register<short> { Address = 1, Value = 1, Name = "Test1", RegisterType = RegisterType.HoldingRegister },
                new Register<short> { Address = 2, Value = 2, Name = "Test2", RegisterType = RegisterType.HoldingRegister }
            };

            serverModel.SwitchTab("HoldingRegisters");

            Assert.Equal(serverModel.HoldingRegisters, serverModel.CurrentItems);

            serverModel = new ModbusServerModel(502);
            serverModel.InputRegisters = new ObservableCollection<IRegister>
            {
                new Register<short> { Address = 1, Value = 1, Name = "Test1", RegisterType = RegisterType.InputRegister },
                new Register<short> { Address = 2, Value = 2, Name = "Test2", RegisterType = RegisterType.InputRegister }
            };

            serverModel.SwitchTab("InputRegisters");
            Assert.Equal(serverModel.InputRegisters, serverModel.CurrentItems);
            serverModel.FilterModifiedRegisters(true);
            Assert.Empty(serverModel.CurrentItems);


        }

    }
}