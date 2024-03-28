using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using TestEase.Models;
using TestEase.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestEase.ViewModels
{
    public partial class MQTTBrokerPageViewModel : ObservableObject, INotifyPropertyChanged
    {
        private MqttBrokerModel _mqttBroker;
        private bool _isBrokerRunning;

        public bool IsBrokerRunning
        {
            get => _isBrokerRunning;
            set => SetProperty(ref _isBrokerRunning, value);
        }



        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public event StatusChangedEventHandler StatusChanged;

        public ObservableCollection<string> ConnectedClients { get; private set; }

        private int _connectCount;
        public int ConnectCount
        {
            get => _connectCount;
            set => SetProperty(ref _connectCount, value);
        }

        private int _disconnectCount;
        public int DisconnectCount
        {
            get => _disconnectCount;
            set => SetProperty(ref _disconnectCount, value);
        }

        public MQTTBrokerPageViewModel()
        {
            _mqttBroker = new MqttBrokerModel();
            IsBrokerRunning = false; // MQTT broker is initially not running
            ConnectedClients = _mqttBroker.ConnectedClients;
            ConnectCount = _mqttBroker.ConnectCount;
            DisconnectCount = _mqttBroker.DisconnectCount;

        }

        public void ToggleCommand(CustomColor greenColor, CustomColor redColor)
        {
            if (IsBrokerRunning)
            {
                _mqttBroker.StopAsync().Wait(); // Wait for the async operation to complete
                IsBrokerRunning = false;
            }
            else
            {
                _mqttBroker.StartAsync().Wait(); // Wait for the async operation to complete
                IsBrokerRunning = true;
            }

            // Raise event to notify view of status change
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(IsBrokerRunning, greenColor, redColor));
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public bool IsRunning { get; }
        public CustomColor GreenColor { get; }
        public CustomColor RedColor { get; }

        public StatusChangedEventArgs(bool isRunning, CustomColor greenColor, CustomColor redColor)
        {
            IsRunning = isRunning;
            GreenColor = greenColor;
            RedColor = redColor;
        }
    }

   
}