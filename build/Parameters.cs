using Nuke.Common;

partial class Build
{
    [Parameter(".NET Runtime ID")]
    readonly string Runtime = "win-x64";

    [Parameter("CUDA Toolkit Version")]
    readonly string CudaVersion = "11.1";
}
