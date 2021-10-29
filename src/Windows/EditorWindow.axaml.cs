using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using DepthEstimateGui.Core;
using DepthEstimateGui.Utils;
using DepthEstimateGui.Utils.UI;
using ReactiveUI;

namespace DepthEstimateGui.Windows
{
    public class EditorWindow : MetroWindow
    {
        public EditorWindow()
        {
            DataContext = new EditorViewModel(this);

            InitializeComponent();

            this.FindControl<Grid>("RootPanel")
                .AddHandler(DragDrop.DropEvent, HandleInputDrop);

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void HandleInputDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Text))
                (DataContext as EditorViewModel)!.HandleDrop(e.Data.GetText());
            else if (e.Data.Contains(DataFormats.FileNames))
                (DataContext as EditorViewModel)!.HandleDrop(
                    (e.Data.GetFileNames() ?? Array.Empty<string>()).FirstOrDefault());
        }

        public static readonly List<string> OutputFormats = new()
        {
            "png",
            "jpg",
            "bmp",
            "dib",
            "eps",
            "gif",
            "apng",
            "tiff",
            "webp"
        };
    }

    public class EditorViewModel : ReactiveObject
    {
        public EditorViewModel(
            MetroWindow view)
        {
            _view = view;

            _isSettingsControlEnabled = this
                .WhenAnyValue(x => x.Graphic!.IsProcessing)
                .Select(x => !x)
                .ToProperty(this, x => x.IsSettingsControlEnabled);

            _isOpenControlEnabled = this
                .WhenAnyValue(x => x.IsSettingsControlEnabled)
                .Select(x => Graphic is null || x)
                .ToProperty(this, x => x.IsOpenControlEnabled);
        }

        #region Core Data

        private Graphic? _graphic;

        public Graphic? Graphic
        {
            get => _graphic;
            set
            {
                if (value == _graphic) return;
                _graphic?.Dispose();
                _graphic = value;
                _graphic?.ProcessComplete.Subscribe(HandleProcessComplete);
                this.RaisePropertyChanged();
            }
        }

        public ProcessSettings Settings { get; } = new();

        private ProcessResult? _result;

        #endregion

        #region View Data

        private readonly MetroWindow _view;

        private Bitmap? _sourceImage;

        public Bitmap? SourceImage
        {
            get => _sourceImage;
            set
            {
                if (value == _sourceImage) return;
                _sourceImage?.Dispose();
                _sourceImage = value;
                this.RaisePropertyChanged();
            }
        }

        private Bitmap? _outputImage;

        public Bitmap? OutputImage
        {
            get => _outputImage;
            set
            {
                if (value == _outputImage) return;
                _outputImage?.Dispose();
                _outputImage = value;
                this.RaisePropertyChanged();
            }
        }

        private string _processOutput = string.Empty;

        public string ProcessOutput
        {
            get => _processOutput;
            set => this.RaiseAndSetIfChanged(ref _processOutput, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isSettingsControlEnabled;

        public bool IsSettingsControlEnabled => _isSettingsControlEnabled.Value;

        private readonly ObservableAsPropertyHelper<bool> _isOpenControlEnabled;

        public bool IsOpenControlEnabled => _isOpenControlEnabled.Value;

        private bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            set => this.RaiseAndSetIfChanged(ref _isCompleted, value);
        }

        private bool _isSucceeded;

        public bool IsSucceeded
        {
            get => _isSucceeded;
            set => this.RaiseAndSetIfChanged(ref _isSucceeded, value);
        }

        #endregion

        #region Command Handlers

        public void HandleDrop(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (Graphic is { IsProcessing: true }) return;
            Graphic = new(path);
            SourceImage = new(Graphic.InputPath);
            OutputImage = null;
            IsCompleted = false;
            IsSucceeded = false;
        }

        public async Task HandleOpen()
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = false,
                Title = "Open Image",
                Filters = FormatFilter.PilOpenFormatFilter
            };

            string[] result = await dialog.ShowAsync((Window)_view.GetVisualRoot());
            if (!result.Any()) return;

            HandleDrop(result[0]);
        }

        public void HandleProcess()
        {
            IsCompleted = false;
            IsSucceeded = false;
            ProcessOutput = string.Empty;
            Graphic!.Process((ProcessSettings)Settings.Clone());
        }

        public void HandleProcessComplete(ProcessResult result)
        {
            _result = result;
            ProcessOutput = result.Summary;
            IsCompleted = true;
            IsSucceeded = result.ExitCode == 0;
            if (IsSucceeded) OutputImage = new(result.OutputPath);
            else OutputImage = null;
        }

        public async Task HandleSave()
        {
            if (_result is null)
                throw new InvalidOperationException("Result is required.");

            string ext = Path.GetExtension(_result.OutputPath)[1..];
            SaveFileDialog dialog = new()
            {
                Filters = new()
                {
                    new()
                    {
                        Name = $"{ext.ToUpper()} (*.{ext})",
                        Extensions = new() { ext }
                    }
                },
                DefaultExtension = ext,
                Title = "Save Image"
            };

            string result = await dialog.ShowAsync((Window)_view.GetVisualRoot());
            if (string.IsNullOrWhiteSpace(result)) return;

            File.Copy(_result.OutputPath, result, true);
        }

        public void HandleShowLog() => LogWindow.ShowLog(_result?.Log ?? string.Empty, _view);

        #endregion
    }
}
