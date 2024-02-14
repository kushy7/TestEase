using EasyModbus;
using System.Runtime.CompilerServices;
using TestEase.Helpers;
using TestEase.Models;
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
        public void ModelTest()
        {
            RegisterModel c = new Coil(2, RegisterType.Coil, "testingRegister");
            Assert.Equal(2, c.Address);
            Assert.Equal(RegisterType.Coil, c.Type);
            Assert.Equal("testingRegister", c.Name);
        }
        [Fact]
        public void randomIntTest()
        {
            int lowerBound = 1;
            int upperBound = 100;
            int result = ValueGenerators.GenerateRandomValueInt(lowerBound, upperBound);
            Assert.InRange(result, lowerBound, upperBound);
        }


        [Fact]
         public void GenerateRandomDoubleTest()
        {
            double lowerBound = 0.0;
            double upperBound = 1.0;
            double result = ValueGenerators.GenerateRandomValueDouble(lowerBound, upperBound);
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
        //public void ServerAcceptingTest()
        //{
        //    ModbusServer mbs = new ModbusServer
        //    {
        //        Port = 502
        //    };


        //}



    }
}