using Microsoft.Extensions.Logging;
using TestEase.ViewModels;
using TestEase.Views;
using CommunityToolkit.Maui;
using TestEase.Services;
using Microsoft.Maui.Platform;
using Microsoft.Maui.LifecycleEvents;

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



            //code to maximize application on startup
#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        Microsoft.UI.WindowId win32WindowsId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        Microsoft.UI.Windowing.AppWindow winuiAppWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(win32WindowsId);
                        if (winuiAppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter p)
                        {
                            p.Maximize();
                        }
                    });
                });
            });
#endif




#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<AppViewModel>();
            builder.Services.AddSingleton<ModbusService>();
            builder.Services.AddSingleton<UpdateService>();
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