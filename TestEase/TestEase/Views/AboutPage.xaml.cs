using TestEase.ViewModels;

namespace TestEase.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}