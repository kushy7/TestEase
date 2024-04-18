using Microsoft.Extensions.Logging;
using TestEase.ViewModels;
using TestEase.Views;
using CommunityToolkit.Maui;
using TestEase.Services;
using Microsoft.Maui.Platform;
using Microsoft.Maui.LifecycleEvents;
using TestEase.WinUI;
using Moq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using Microsoft.UI.Windowing;

namespace TestEase
{
    public static class MauiProgram
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();



            //code to maximize application on startup and save server info 
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
                        // ON APP CLOSE, SAVE SERVERS AND THEIR CONFIGS
                        window.AppWindow.Closing += (o, e) =>
                        {
                            OnAppStopping(o, e);
                        };
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

            var app = builder.Build();

            ServiceProvider = app.Services; // Storing the service provider

            // Load the servers from local json
            // Located somewhere like this: C:\Users\<name>\AppData\Local\Packages\com.companyname.testease_9zz4h110yvjzm\LocalState
            OnAppStarting();

            return app;
        }

        private static void OnAppStopping(object sender, AppWindowClosingEventArgs e)
        {
            var appViewModel = MauiProgram.ServiceProvider.GetService<AppViewModel>();
            if (appViewModel != null)
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "servers.json");
                appViewModel.SaveServers(filePath);
            }
        }

        private static void OnAppStarting()
        {
            var appViewModel = MauiProgram.ServiceProvider.GetService<AppViewModel>();
            if (appViewModel != null)
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "servers.json");
                appViewModel.LoadServers(filePath);
            }
        }
    }
}