using System;
using System.IO;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public partial class Graphic
    {
        private bool _isProcessing;

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        public void Process(ProcessSettings settings)
        {
            if (IsProcessing)
                throw new InvalidOperationException("Already processing.");

            IsProcessing = true;
            Task.Run(() => ProcessIntl(settings));
        }

        private readonly Subject<ProcessResult> _processComplete = new();

        public IObservable<ProcessResult> ProcessComplete => _processComplete;

        private async Task ProcessIntl(ProcessSettings settings)
        {
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            // Prepare paths
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string pythonPath = Path.Combine(
                rootPath,
                isWindows ?
                    "tools\\python.exe" :
                    "tools/bin/python");
            string corePath = Path.Combine(rootPath, "tools", settings.Core);
            string outputName = $"{Id}.{settings.Ext}";

            GraphicStorage.EnsureDirectoryExists();

            // Start process
            StringBuilder output = new();
            CommandResult result = await Cli
                .Wrap(pythonPath)
                .WithArguments(
                    $"{(isWindows ? "" : "./")}1kgen.py {InputName} {outputName} {settings.ColorMap}")
                .WithWorkingDirectory(corePath)
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(output))
                .ExecuteAsync();

            IsProcessing = false;
            _processComplete.OnNext(new(GraphicStorage.GetOutputFilePath(outputName), output.ToString(),
                result.ExitCode, result.RunTime));
        }
    }
}
