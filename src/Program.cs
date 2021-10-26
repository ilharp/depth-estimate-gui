using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DepthEstimateGui
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Magic - github.com/AvaloniaUI/Avalonia/issues/1934
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            // Build Avalonia app
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .LogToTrace();
    }
}
