using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

public class MqttBroker : BindableBase
{
    private MqttServer mqttServer;
    private ObservableCollection<string> _messages = new ObservableCollection<string>();

    public ObservableCollection<string> Messages
    {
        get { return _messages; }
        set { SetProperty(ref _messages, value); }
    }

    public async Task Start()
    {
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpointPort(1883).Build();
        
        mqttServer = new MqttFactory().CreateMqttServer();

        mqttServer.ClientConnectedAsync();

        mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(e =>
        {
            AddMessage($"Client disconnected: {e.ClientId}");
        });

        mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
        {
            AddMessage($"Message received: Topic={e.ApplicationMessage.Topic}, Payload={Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
        });

        await mqttServer.StartAsync(optionsBuilder.Build());
    }

    public async Task Stop()
    {
        await mqttServer.StopAsync();
    }

    private void AddMessage(string message)
    {
        Messages.Add(message);
        OnPropertyChanged(nameof(Messages));
    }
}