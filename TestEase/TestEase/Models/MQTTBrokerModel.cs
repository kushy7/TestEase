using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;


//each broker keeps track of clients, messages, and client connect start time
public class MqttBrokerModel : INotifyPropertyChanged
{
    private IMqttServer mqttServer;
    private int _connectCount;
    private int _disconnectCount;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<string>? ClientDisconnected;

    public ObservableCollection<string> ConnectedClients { get; private set; }
    public ObservableCollection<string> ReceivedMessages { get; private set; }
    private Dictionary<string, DateTime> ClientConnectionStartTimes { get; set; }
    public Dictionary<string, int> ClientMessagesSent { get; set; }

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
        ReceivedMessages = new ObservableCollection<string>();
        ClientConnectionStartTimes = new Dictionary<string, DateTime>();
        ClientMessagesSent = new Dictionary<string, int>();

        mqttServer = new MqttFactory().CreateMqttServer();
        mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ConnectCount++;
                OnPropertyChanged(nameof(ConnectCount));
                ConnectedClients.Add(e.ClientId);
                ClientMessagesSent.Add(e.ClientId, 0);
                ClientConnectionStartTimes[e.ClientId] = DateTime.UtcNow;

            });
        });

        mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DisconnectCount++;
                OnPropertyChanged(nameof(DisconnectCount));
                ConnectedClients.Remove(e.ClientId);
                ClientMessagesSent.Remove(e.ClientId);
                ClientConnectionStartTimes.Remove(e.ClientId);
                ClientDisconnected?.Invoke(this, e.ClientId);

            });
        });
        mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Add messages to the top
                if (e.ApplicationMessage.Payload == null)
                {
                    e.ApplicationMessage.Payload = new byte[0];
                }
               
                ReceivedMessages.Insert(0, $"[{DateTime.Now}] {e.ClientId}:\n{e.ApplicationMessage.Topic}\n{System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                
                // Increment client message count
                int currentCount;
                ClientMessagesSent.TryGetValue(e.ClientId, out currentCount);
                ClientMessagesSent[e.ClientId] = currentCount + 1;
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



    public TimeSpan GetClientConnectionUptime(string clientId)
    {
        if (clientId != null && ClientConnectionStartTimes.TryGetValue(clientId, out DateTime connectionStartTime))
        {
            return DateTime.UtcNow - connectionStartTime;
        }
        else
        {
            return TimeSpan.Zero; // Return zero if client not found or not currently connected
        }

        
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
