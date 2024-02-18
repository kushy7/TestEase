using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using EasyModbus;
using TestEase.Models;
using TestEase.ViewModels;
//using Windows.Graphics.Printing3D;

namespace TestEase.Services
{
    public class ModbusService(AppViewModel appViewModel)
    {
        // Manages the servers through the EasyModbus model
        // public List<ModbusServer> modbusServers = new List<ModbusServer>();

        // Manages the servers through the ModbusModel
        // public List<ModbusServerModel> modbusModels = new List<ModbusServerModel>();

        public void CreateServer(int port)
        {
            appViewModel.ModbusServers.Add(new ModbusServerModel(port));
        }
        public void StartServer(int port)
        {
            var server = appViewModel.ModbusServers.FirstOrDefault(s => s.Port == port);
            if (server != null)
                server.Server.Listen();
        }

        public void StopServer(int port)
        {
            var server = appViewModel.ModbusServers.FirstOrDefault(s => s.Port == port);
            if (server != null)
                server.Server.StopListening();
        }

        public short ReadHoldingRegister(int port, int address)
        {
            var server = appViewModel.ModbusServers.FirstOrDefault(s => s.Port == port);
            if (server != null)
                return server.Server.holdingRegisters[address];
            else return 0;
        }
        public void WriteHoldingRegister(int port, int address, short value)
        {
            var server = appViewModel.ModbusServers.FirstOrDefault(s => s.Port == port);
            if (server != null)
                server.Server.holdingRegisters[address] = value;
        }

        public short[] GetHoldingRegisters(int port)
        {
            //var server = modbusServers.FirstOrDefault(s => s.Port == port);
            //if (server != null)
             //   return server.holdingRegisters.localArray;
            //return null;
            var server = appViewModel.ModbusServers.FirstOrDefault(s => s.Port == port);
            if (server != null)
                return server.Server.holdingRegisters.localArray;
            return [0];
        }

    }
}
