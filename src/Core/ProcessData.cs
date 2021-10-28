using System;
using System.Linq;
using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public class ProcessSettings : ReactiveObject, ICloneable
    {
        public ProcessSettings(
            string core = "MiDaS",
            ColorMap? colorMap = null,
            string ext = "png")
        {
            _core = core;
            _colorMap = colorMap ?? ColorMap.MapList.Single(x => x.Name == "Greys_r");
            _ext = ext;
        }

        private string _core;

        public string Core
        {
            get => _core;
            set => this.RaiseAndSetIfChanged(ref _core, value);
        }

        private ColorMap _colorMap;

        public ColorMap ColorMap
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
            string log,
            int exitCode,
            TimeSpan runTime)
        {
            OutputPath = outputPath;
            Log = log;
            ExitCode = exitCode;
            RunTime = runTime;
        }

        public readonly string OutputPath;
        public readonly string Log;
        public readonly int ExitCode;
        public readonly TimeSpan RunTime;

        public string Summary => $"{(ExitCode == 0 ? "Completed" : "Error occurred")} in {RunTime}";
    }
}
