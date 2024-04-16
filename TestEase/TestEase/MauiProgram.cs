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
                        window.AppWindow.Closing += (o, e) =>
                        {
                            Trace.WriteLine("EXITING!");
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
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ModbusPageViewModel>();
            builder.Services.AddTransient<ModbusPage>();
            builder.Services.AddTransient<MQTTBrokerPageViewModel>();
            builder.Services.AddTransient<MQTTBrokerPage>();
            builder.Services.AddTransient<AboutPageViewModel>();
            builder.Services.AddTransient<AboutPage>();

            var app = builder.Build();

            return app;
        }

        private static void OnAppStopping(object sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs e)
        {
            Trace.WriteLine("EXITING possibly?");
            Trace.WriteLine(sender);
            if (sender is MauiApp app2)
            {
                Trace.WriteLine("EXITING maybe?");
                var appViewModel = app2.Services.GetService<AppViewModel>();
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "servers.json");
                appViewModel?.SaveServers(filePath);
                Trace.WriteLine(filePath);
                Trace.WriteLine("EXITING FOR REAL!");
                Process.Start("explorer.exe", filePath);
            }
            if (sender is Microsoft.UI.Windowing.AppWindow app)
            {
                Trace.WriteLine("EXITING maybe?");
                var appViewModel = app.Services.GetService<AppViewModel>();
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "servers.json");
                appViewModel?.SaveServers(filePath);
                Trace.WriteLine(filePath);
                Trace.WriteLine("EXITING FOR REAL!");
                Process.Start("explorer.exe", filePath);
            }
        }
    }
}