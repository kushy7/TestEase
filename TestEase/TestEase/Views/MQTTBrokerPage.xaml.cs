using TestEase.ViewModels;

namespace TestEase.Views;

public partial class MQTTBrokerPage : ContentPage
{
	public MQTTBrokerPage(MQTTBrokerPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}