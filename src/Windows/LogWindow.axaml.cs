using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using DepthEstimateGui.Utils.UI;
using ReactiveUI;

namespace DepthEstimateGui.Windows
{
    public class LogWindow : MetroWindow
    {
        public LogWindow()
        {
            throw new InvalidOperationException("Please use LogWindow.ShowLog().");
        }

        private LogWindow(string log)
        {
            InitializeComponent();

            DataContext = new LogViewModel(this, log);

#if DEBUG
            this.AttachDevTools();
#endif
        }

        public static Task ShowLog(string log, Window? owner) =>
            new LogWindow(log).ShowDialog(owner);

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class LogViewModel : ReactiveObject
    {
        public LogViewModel(MetroWindow view, string log)
        {
            _view = view;
            Log = string.IsNullOrWhiteSpace(log) ? string.Empty : log;
        }

        private readonly MetroWindow _view;

        public string Log { get; }

        public async Task HandleSave()
        {
            SaveFileDialog dialog = new()
            {
                Title = "Save Log",
                DefaultExtension = "log",
                InitialFileName = $"depth-estimate-log-{DateTime.Now:MM-dd-hh-mm-ss}.log",
                Filters = new()
                {
                    new() { Name = "Log", Extensions = new() { "log" } },
                    new() { Name = "Text", Extensions = new() { "txt" } }
                }
            };

            string result = await dialog.ShowAsync((Window)_view.GetVisualRoot());

            if (string.IsNullOrWhiteSpace(result)) return;

            await File.WriteAllTextAsync(
                result,
                Log);
        }
    }
}
