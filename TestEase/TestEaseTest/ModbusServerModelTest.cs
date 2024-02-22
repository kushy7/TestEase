using EasyModbus;
using System.Runtime.CompilerServices;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.Services;
using Xunit.Sdk;
using Moq;

namespace TestEaseTest
{
    public class ModbusServerTest
    {
        [Fact]
        public void testReadWriteHoldingRegister()
        {
            ModbusServer server = new ModbusServer();
            ModbusServerModel serverModel = new ModbusServerModel(502);
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
            ModbusServer server = new ModbusServer();
            ModbusServerModel serverModel = new ModbusServerModel(502);
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
            ModbusServer server = new ModbusServer();
            ModbusServerModel serverModel = new ModbusServerModel(502);
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
            ModbusServer server = new ModbusServer();
            ModbusServerModel serverModel = new ModbusServerModel(502);
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

    }
}