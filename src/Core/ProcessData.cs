using System;
using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public class ProcessSettings : ReactiveObject, ICloneable
    {
        public ProcessSettings(
            string core = "MiDaS",
            string colorMap = "Greys_r",
            string ext = "png")
        {
            _core = core;
            _colorMap = colorMap;
            _ext = ext;
        }

        private string _core;

        public string Core
        {
            get => _core;
            set => this.RaiseAndSetIfChanged(ref _core, value);
        }

        private string _colorMap;

        public string ColorMap
        {
            get => _colorMap;
            set => this.RaiseAndSetIfChanged(ref _colorMap, value);
        }

        private string _ext;

        public string Ext
        {
            get => _ext;
            set => this.RaiseAndSetIfChanged(ref _ext, value);
        }

        public object Clone() => new ProcessSettings(Core, ColorMap, Ext);
    }

    public class ProcessResult
    {
        public ProcessResult(
            string outputPath,
            string output,
            int exitCode,
            TimeSpan runTime)
        {
            OutputPath = outputPath;
            Output = output;
            ExitCode = exitCode;
            RunTime = runTime;
        }

        public readonly string OutputPath;
        public readonly string Output;
        public readonly int ExitCode;
        public readonly TimeSpan RunTime;

        public string Summary => $"{(ExitCode == 0 ? "Completed" : "Error occurred")} in {RunTime}";
    }
}
