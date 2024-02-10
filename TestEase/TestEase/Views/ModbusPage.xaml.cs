using TestEase.ViewModels;

namespace TestEase.Views;

public partial class ModbusPage : ContentPage
{
	public ModbusPage(ModbusPageViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;
    }
}