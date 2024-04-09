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

        // field to set and get for if the broker is actively running
        public bool IsBrokerRunning
        {
            get => _isBrokerRunning;
            set => SetProperty(ref _isBrokerRunning, value);
        }

        //field for if a client has been clicked from the list of actively connected clients
        private string _selectedClient;
        public string SelectedClient
        {
            get => _selectedClient;
            set
            {
                //once selected client has been set, triggers property changed event for
                //selectedclient, if the client has been selected, and if their info should be displayed
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(IsClientSelected));
                OnPropertyChanged(nameof(SelectedClientInfo));

                //if not empty actively update the client uptime timer
                if (!string.IsNullOrEmpty(value))
                {
                    ClientConnectionUptime = _mqttBroker.GetClientConnectionUptime(value);
                    ClientMessagesSent = _mqttBroker.ClientMessagesSent[SelectedClient];
                    _updateTimer.Start();
                //otherwise stop
                } else
                {
                    _updateTimer.Stop();
                }
                
            }
        }


        //check to see if there is a client selected
        public bool IsClientSelected => !string.IsNullOrEmpty(SelectedClient);
        //get info for the selected client
        public string SelectedClientInfo
        {
            get
            {
                // Return client details based on the selected client
                return $"Client Info for {SelectedClient}";
            }
        }
        //gets and sets the uptime for a connected client, activley updating
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
        //tracks how many messages have been sent by the client, gets values from MQTTBrokerModel
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
        //list of connected clients
        public ObservableCollection<string> ConnectedClients { get; private set; }
        //list of received messages
        public ObservableCollection<string> ReceivedMessages { get; private set; }
        //actively updating timer for client uptime
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
        //search text to be used for filtering through messages
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
            //instantiates a new MQTTBrokerModel
            _mqttBroker = new MqttBrokerModel();
            IsBrokerRunning = false; // MQTT broker is initially not running
            //grabs the connected clients from the model
            ConnectedClients = _mqttBroker.ConnectedClients;
            //grabs the received messages from the model
            ReceivedMessages = _mqttBroker.ReceivedMessages;
            //checks and actively updates the connect and disonnect counts whenever a connection
            //or disconnection occurs
            _mqttBroker.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(ConnectCount));
                OnPropertyChanged(nameof(DisconnectCount));
            };
            //reset selected client to null if disconnected
            _mqttBroker.ClientDisconnected += (sender, client) =>
            {
                if (SelectedClient == client)
                {
                    SelectedClient = ""; // Set SelectedClient to empty string
                }
            };
            //handles updating client uptime connection
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
        //stops ends the timer updating
        public void Dispose()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
        }
        //handles filtering messages based on text put into the search bar
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
        //starts or stops the broker depending on when the button is clicked and raises event to change color
        //of the ellipse based on whether it is on or off
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
    //handles changing of colors for the ellipse when broker is on or off
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