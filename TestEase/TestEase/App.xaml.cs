using Microsoft.Maui.Platform;
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

            MainPage = new AppShell() { BindingContext = appViewModel };
        }

        //protected override Window CreateWindow(IActivationState activationState)
        //{
        //    var window = base.CreateWindow(activationState);

        //    const int newWidth = 1920;
        //    const int newHeight = 1080;

        //    window.Width = newWidth;
        //    window.Height = newHeight;

        //    window.X = 0;
        //    window.Y = 0;

        //    return window;
        //}
    }
}
