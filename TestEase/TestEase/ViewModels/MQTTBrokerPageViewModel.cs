using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using TestEase.Models;
using TestEase.Helpers;

namespace TestEase.ViewModels
{
    public partial class MQTTBrokerPageViewModel : ObservableObject
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

        public MQTTBrokerPageViewModel()
        {
            _mqttBroker = new MqttBrokerModel();
            IsBrokerRunning = false; // MQTT broker is initially not running
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