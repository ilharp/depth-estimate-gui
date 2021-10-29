using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
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
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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

            _isControlEnabled = this
                .WhenAnyValue(x => x.Graphic!.IsProcessing)
                .Select(x => !x)
                .ToProperty(this, x => x.IsControlEnabled);
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
                if (_graphic is not null)
                    _graphic.ProcessComplete.Subscribe(HandleProcessComplete);
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

        private readonly ObservableAsPropertyHelper<bool> _isControlEnabled;

        public bool IsControlEnabled => _isControlEnabled.Value;

        private bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            set => this.RaiseAndSetIfChanged(ref _isCompleted, value);
        }

        #endregion

        #region Command Handlers

        public async Task HandleOpen()
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = false,
                Title = "Open Image",
                Filters = FormatFilter.PilOpenFormatFilter
            };

            string[] result = await dialog.ShowAsync((Window)_view.GetVisualRoot());
            if (!result.Any() ||
                string.IsNullOrWhiteSpace(result[0])) return;

            Graphic = new(result[0]);

            // Load original image
            SourceImage = new(Graphic.InputPath);
        }

        public void HandleProcess()
        {
            IsCompleted = false;
            ProcessOutput = string.Empty;
            Graphic!.Process((ProcessSettings)Settings.Clone());
        }

        public void HandleProcessComplete(ProcessResult result)
        {
            _result = result;
            ProcessOutput = result.Summary;
            IsCompleted = true;
            OutputImage = new(result.OutputPath);
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

        #endregion
    }
}
