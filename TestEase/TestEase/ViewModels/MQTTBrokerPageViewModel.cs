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
        public int ConnectCount => _mqttBroker.ConnectCount;
        public int DisconnectCount => _mqttBroker.DisconnectCount;

        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan _clientConnectionUptime;

        public bool IsBrokerRunning
        {
            get => _isBrokerRunning;
            set => SetProperty(ref _isBrokerRunning, value);
        }


        private string _selectedClient;
        public string SelectedClient
        {
            get => _selectedClient;
            set
            {
                SetProperty(ref _selectedClient, value);
                OnPropertyChanged(nameof(IsClientSelected));
                OnPropertyChanged(nameof(SelectedClientInfo));
                ClientConnectionUptime = _mqttBroker.GetClientConnectionUptime(value);
            }
        }

        public bool IsClientSelected => !string.IsNullOrEmpty(SelectedClient);

        public string SelectedClientInfo
        {
            get
            {
                // Return client details based on the selected client
                return $"Client Info for {SelectedClient}";
            }
        }

        public TimeSpan ClientConnectionUptime
        {
            get => _clientConnectionUptime;
            set
            {
                if (_clientConnectionUptime != value)
                {
                    _clientConnectionUptime = value;
                    OnPropertyChanged(nameof(ClientConnectionUptime));
                }
            }
        }



        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public event StatusChangedEventHandler StatusChanged;

        public ObservableCollection<string> ConnectedClients { get; private set; }

        public ObservableCollection<string> ReceivedMessages { get; private set; }





        public MQTTBrokerPageViewModel()
        {
            _mqttBroker = new MqttBrokerModel();
            IsBrokerRunning = false; // MQTT broker is initially not running
            ConnectedClients = _mqttBroker.ConnectedClients;
            ReceivedMessages = _mqttBroker.ReceivedMessages;


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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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