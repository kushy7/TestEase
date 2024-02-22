using EasyModbus;
using System.Runtime.CompilerServices;
using TestEase.Helpers;
using TestEase.Models;
using TestEase.Services;
using Xunit.Sdk;

namespace TestEaseTest
{
    public class UnitTest1
    {
        [Fact]
        public void InitialTestingTest()
        {
        
            ModbusServerModel modbus = new ModbusServerModel(502);
            Assert.Equal(502, modbus.Port);
            Assert.False(modbus.IsRunning);

        }

        [Fact]
        public void coilOrDiscreteTest()
        {
            CoilOrDiscrete c = new CoilOrDiscrete(2, RegisterType.Coil, "testingRegister", false);
            Assert.Equal(2, c.Address);
            Assert.Equal(RegisterType.Coil, c.Type);
            Assert.Equal("testingRegister", c.Name);
            Assert.False(c.value);
        }

        [Fact]
        public void randomShortTest()
        {
            short lowerBound = 1;
            short upperBound = 100;
            short result = ValueGenerators.GenerateRandomValueShort(lowerBound, upperBound);
            Assert.InRange(result, lowerBound, upperBound);
        }


        [Fact]
         public void GenerateRandomDoubleTest()
        {
            double lowerBound = 0.0;
            double upperBound = 1.0;
            float result = ValueGenerators.GenerateRandomValueFloat((float)lowerBound, (float)upperBound);
            Assert.InRange(result, lowerBound, upperBound);
        }

        [Fact]
        public void FixedTest()
        {
            Fixed<short> f = new Fixed<short>(2, RegisterType.Coil, "testingRegister", 44);
            Assert.Equal(2, f.Address);
            Assert.Equal(RegisterType.Coil, f.Type);
            Assert.Equal("testingRegister", f.Name);
            Assert.Equal(44, f.value);
 
        }

        //[Fact]
        //public void ServiceCreateServerTest()
        //{
        //    ModbusService service = new ModbusService();
        //    service.CreateServer(502);
        //    Assert.Single(service.modbusServers);
        //}

        //[Fact]
        //public void ServiceEditRegisterTest()
        //{
        //    ModbusService service = new ModbusService();
        //    service.CreateServer(502);
        //    Assert.Single(service.modbusServers);
        //    service.WriteHoldingRegister(502, 5, 25);
        //    Assert.Equal(25, service.ReadHoldingRegister(502, 5));
        //}

   



    }
}