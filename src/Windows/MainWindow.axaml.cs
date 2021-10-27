using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml;
using DepthEstimateGui.Utils.UI;
using ReactiveUI;

namespace DepthEstimateGui.Windows
{
    public class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class MainWindowViewModel : ReactiveObject
    {
        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString()!;
    }
}
