using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Configuration;

using MQTTnet;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;

    public class MqttBrokerModel
    {
        private MqttServer? mqttServer;

        public async Task StartAsync()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883);

            var mqttServerOptions = optionsBuilder.Build();

            mqttServer = new MqttFactory().CreateMqttServer(mqttServerOptions);

            await mqttServer.StartAsync();
        }


        public async Task StopAsync()
        {
            if (mqttServer != null)
            {
                await mqttServer.StopAsync();
                mqttServer.Dispose();
                mqttServer = null;
            }
        }
    }
