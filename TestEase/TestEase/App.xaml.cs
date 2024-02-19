using TestEase.Services;
using TestEase.ViewModels;

namespace TestEase
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //Application.Current.UserAppTheme = AppTheme.Light;
            var appViewModel = new AppViewModel();

            var modbusService = serviceProvider.GetService<ModbusService>();
            modbusService.StartPeriodicUpdate(TimeSpan.FromSeconds(3));

            MainPage = new AppShell() { BindingContext = appViewModel};
        }
    }
}
