using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

public class MqttBrokerModel : INotifyPropertyChanged
{
    private IMqttServer mqttServer;
    private int _connectCount;
    private int _disconnectCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> ConnectedClients { get; private set; }

    public int ConnectCount
    {
        get => _connectCount;
        set
        {
            if (_connectCount != value)
            {
                _connectCount = value;
                OnPropertyChanged(nameof(ConnectCount));
            }
        }
    }

    public int DisconnectCount
    {
        get => _disconnectCount;
        set
        {
            if (_disconnectCount != value)
            {
                _disconnectCount = value;
                OnPropertyChanged(nameof(DisconnectCount));
            }
        }
    }


    [Obsolete]
    public MqttBrokerModel()
    {
        ConnectedClients = new ObservableCollection<string>();
        mqttServer = new MqttFactory().CreateMqttServer();
        mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ConnectCount++;
                OnPropertyChanged(nameof(ConnectCount));
                ConnectedClients.Add("Client: " + e.ClientId);
                
            });
        });

        mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DisconnectCount++;
                OnPropertyChanged(nameof(DisconnectCount));
                ConnectedClients.Remove("Client: " + e.ClientId);
                
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

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
