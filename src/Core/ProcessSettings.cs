using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public class ProcessSettings : ReactiveObject
    {
        private string _core = "midas";

        public string Core
        {
            get => _core;
            set => this.RaiseAndSetIfChanged(ref _core, value);
        }

        private string _colorMap = "Greys_r";

        public string ColorMap
        {
            get => _colorMap;
            set => this.RaiseAndSetIfChanged(ref _colorMap, value);
        }
    }
}
