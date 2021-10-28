using System;
using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public class ProcessSettings : ReactiveObject, ICloneable
    {
        public ProcessSettings(
            string core,
            string colorMap,
            string ext)
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
}
