using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

public class MqttBrokerModel
{
    private IMqttServer mqttServer;
    private List<string> connectedClients;

    public MqttBrokerModel()
    {
        connectedClients = new List<string>();
        mqttServer = new MqttFactory().CreateMqttServer();
        mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e =>
        {
            lock (connectedClients)
            {
                connectedClients.Add(e.ClientId);
            }
        });

        mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e =>
        {
            lock (connectedClients)
            {
                connectedClients.Remove(e.ClientId);
            }
        });
    }

    public async Task StartAsync()
    {
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883);

        var mqttServerOptions = optionsBuilder.Build();

        await mqttServer.StartAsync(mqttServerOptions);
    }

    public async Task StopAsync()
    {
        await mqttServer.StopAsync();
    }

    public List<string> GetConnectedClients()
    {
        lock (connectedClients)
        {
            return new List<string>(connectedClients);
        }
    }

    public int GetConnectedClientsCount()
    {
        lock (connectedClients)
        {
            return connectedClients.Count;
        }
    }
}
