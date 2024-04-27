
using System.Diagnostics;
using TestEase.Services;
using TestEase.ViewModels;


namespace TestEase.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}


    //theme selector that saves the users prefrence to local storage for future sessions
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
        ConfigurationService cs = new ConfigurationService();
        Directory.CreateDirectory(Path.Combine(cs.GetFolderPath(), "session"));
        Process.Start("explorer.exe", Path.Combine(cs.GetFolderPath(), "session"));
    }

}