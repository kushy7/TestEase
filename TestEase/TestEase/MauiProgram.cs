using Microsoft.Extensions.Logging;
using TestEase.ViewModels;
using TestEase.Views;
using CommunityToolkit.Maui;
using TestEase.Services;

namespace TestEase
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
            
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<AppViewModel>();
            builder.Services.AddSingleton<ModbusService>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ModbusPageViewModel>();
            builder.Services.AddTransient<ModbusPage>();
            builder.Services.AddTransient<MQTTBrokerPageViewModel>();
            builder.Services.AddTransient<MQTTBrokerPage>();
            builder.Services.AddTransient<AboutPageViewModel>();
            builder.Services.AddTransient<AboutPage>();
            return builder.Build();
        }
    }
}