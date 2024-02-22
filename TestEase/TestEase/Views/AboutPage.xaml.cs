using Microsoft.UI.Windowing;
using TestEase.ViewModels;
using Windows.UI.WindowManagement;

namespace TestEase.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}

    private void OnTogged(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}