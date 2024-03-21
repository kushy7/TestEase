using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestEase.Models;

namespace TestEase.ViewModels
{
    public partial class MQTTBrokerPageViewModel: ObservableObject
    {
        private MqttBrokerModel mqttBroker;



        public MQTTBrokerPageViewModel()
        {
            mqttBroker = new MqttBrokerModel();

            
        }

        public async void StartCommand()
        {
            await mqttBroker.StartAsync();
        }
        
    }
}
