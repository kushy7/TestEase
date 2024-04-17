
using System.Diagnostics;
using TestEase.ViewModels;


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
            Preferences.Set("AppTheme", "Dark");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set("AppTheme", "Light");
        }
    }

    private void OpenFileExplorer(object sender, EventArgs e)
    {
        Process.Start("explorer.exe", FileSystem.AppDataDirectory);
    }

}