using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

#pragma warning disable CA1806

// ReSharper disable InconsistentNaming

namespace DepthEstimateGui.Utils.UI
{
    public class MetroWindow : Window
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                UseImmersiveDarkMode();
        }

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private void UseImmersiveDarkMode()
        {
            if (!IsWindows10OrGreater(17763)) return;

            int attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            if (IsWindows10OrGreater(18985)) attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;

            int useImmersiveDarkMode = 1;

            DwmSetWindowAttribute(PlatformImpl.Handle.Handle, attribute, ref useImmersiveDarkMode, sizeof(int));
        }

        private static bool IsWindows10OrGreater(int build = -1) =>
            Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
    }
}
