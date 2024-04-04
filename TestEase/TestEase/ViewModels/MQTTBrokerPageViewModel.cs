using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TestEase.Models;
using TestEase.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using System.Timers;
using System.Diagnostics;

namespace TestEase.ViewModels
{
    public partial class MQTTBrokerPageViewModel : ObservableObject, INotifyPropertyChanged
    {
        private MqttBrokerModel _mqttBroker;
        private bool _isBrokerRunning;
        public int ConnectCount => _mqttBroker.ConnectCount;
        public int DisconnectCount => _mqttBroker.DisconnectCount;

        public event PropertyChangedEventHandler PropertyChanged;

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
                // SetProperty(ref _selectedClient, value);
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(IsClientSelected));
                OnPropertyChanged(nameof(SelectedClientInfo));

                if (!string.IsNullOrEmpty(value))
                {
                    ClientConnectionUptime = _mqttBroker.GetClientConnectionUptime(value);
                    ClientMessagesSent = _mqttBroker.ClientMessagesSent[SelectedClient];
                    _updateTimer.Start();
                } else
                {
                    _updateTimer.Stop();
                }
                
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

        private TimeSpan _clientConnectionUptime;
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

        private int _clientMessagesSent;
        public int ClientMessagesSent
        {
            get => _clientMessagesSent;
            set
            {
                if (_clientMessagesSent != value)
                {
                    _clientMessagesSent = value;
                    OnPropertyChanged(nameof(ClientMessagesSent));
                }
            }
        }



        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public event StatusChangedEventHandler StatusChanged;

        public ObservableCollection<string> ConnectedClients { get; private set; }

        public ObservableCollection<string> ReceivedMessages { get; private set; }

        private System.Timers.Timer _updateTimer;

        // For filtering messages
        private ObservableCollection<string> _filteredMessages;

        public ObservableCollection<string> FilteredMessages
        {
            get => _filteredMessages;
            private set
            {
                _filteredMessages = value;
                OnPropertyChanged(nameof(FilteredMessages));
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterMessages();
            }
        }

        public MQTTBrokerPageViewModel()
        {
            _mqttBroker = new MqttBrokerModel();
            IsBrokerRunning = false; // MQTT broker is initially not running
            ConnectedClients = _mqttBroker.ConnectedClients;
            ReceivedMessages = _mqttBroker.ReceivedMessages;

            _mqttBroker.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(ConnectCount));
                OnPropertyChanged(nameof(DisconnectCount));
            };

            _updateTimer = new System.Timers.Timer(1000);
            _updateTimer.Elapsed += UpdateTimerElapsed;
            _updateTimer.AutoReset = true;
        }

        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedClient) && ConnectedClients.Contains(SelectedClient))
            {
                ClientConnectionUptime = _mqttBroker.GetClientConnectionUptime(SelectedClient);
                ClientMessagesSent = _mqttBroker.ClientMessagesSent[SelectedClient];
            }
        }

        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }

        private void FilterMessages()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                // FilteredMessages = new ObservableCollection<string>(ReceivedMessages);
                FilteredMessages = ReceivedMessages;
            }
            else
            {
                var filtered = ReceivedMessages.Where(message => message.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                FilteredMessages = new ObservableCollection<string>(filtered);
            }
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