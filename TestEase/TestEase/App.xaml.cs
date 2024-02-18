using TestEase.ViewModels;

namespace TestEase
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Application.Current.UserAppTheme = AppTheme.Light;
            var appViewModel = new AppViewModel();
            MainPage = new AppShell() { BindingContext = appViewModel};
        }
    }
}
