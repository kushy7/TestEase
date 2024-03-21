using TestEase.ViewModels;

namespace TestEase.Views;

public partial class MQTTBrokerPage : ContentPage
{
    public MQTTBrokerPage(MQTTBrokerPageViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }

    public void StartCommand(object sender, EventArgs e)
    {
        var vm = this.BindingContext as MQTTBrokerPageViewModel;
        vm.StartCommand();
    }

}