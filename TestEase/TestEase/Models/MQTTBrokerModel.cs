using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

public class MqttBrokerModel
{
    private IMqttServer mqttServer;
    public ObservableCollection<string> ConnectedClients { get; private set; }

    private int _connectCount = 0;
    private int _disconnectCount = 0;

    public int ConnectCount => _connectCount;
    public int DisconnectCount => _disconnectCount;

    [Obsolete]
    public MqttBrokerModel()
    {
        ConnectedClients = new ObservableCollection<string>();
        mqttServer = new MqttFactory().CreateMqttServer();
        mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ConnectedClients.Add("Client: " + e.ClientId);
                _connectCount++;
            });
        });

        mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ConnectedClients.Remove("Client: " + e.ClientId);
                _disconnectCount++;
            });
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

    public ObservableCollection<string> GetConnectedClients()
    {
        lock (ConnectedClients)
        {
            return new ObservableCollection<string>(ConnectedClients);
        }
    }

    public int GetConnectedClientsCount()
    {
        lock (ConnectedClients)
        {
            return ConnectedClients.Count;
        }
    }
}
