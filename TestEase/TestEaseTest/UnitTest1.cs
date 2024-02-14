using TestEase.Models;
using Xunit.Sdk;

namespace TestEaseTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        
            ModbusServerModel modbus = new ModbusServerModel(502);
            Assert.Equal(502, modbus.Port);
            Assert.False(modbus.IsRunning);

        }
    }
}