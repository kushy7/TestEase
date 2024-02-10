using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyModbus;
using TestEase.Models;
using Windows.Graphics.Printing3D;

namespace TestEase.Services
{
    public class ModbusService
    {
        // Manages the servers through the EasyModbus model
        private List<ModbusServer> modbusServers = new List<ModbusServer>();

        // Manages the servers through the ModbusModel
        private List<ModbusModel> modbusModels = new List<ModbusModel>();

        public void CreateServer(int port)
        {
            ModbusServer modbusServer = new ModbusServer
            {
                Port = port
            };
            modbusServers.Add(modbusServer);

            ModbusModel modbusModel = new ModbusModel(port);
            modbusModels.Add(modbusModel);
        }
        public void StartServer(int port)
        {
            var server = modbusServers.FirstOrDefault(s => s.Port == port);
            server?.Listen();
        }

        public void StopServer(int port)
        {
            var server = modbusServers.FirstOrDefault(s => s.Port == port);
            server?.StopListening();
        }

        public int ReadHoldingRegister(int serverPort, int address)
        {
            var server = modbusServers.FirstOrDefault(s => s.Port == serverPort);
            return server?.holdingRegisters[address] ?? 0;
        }
        public void WriteHoldingRegister(int serverPort, int address, int value)
        {
            var server = modbusServers.FirstOrDefault(s => s.Port == serverPort);
            var model = modbusModels.FirstOrDefault(m => m.Port == serverPort);
            if (server != null)
            {
                // Int16/short limited to -32768 to 32767
                server.holdingRegisters[address] = (short)value;
            }
        }

    }
}
