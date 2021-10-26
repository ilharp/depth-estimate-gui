using System.IO.Compression;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    Target Publish => _ => _
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(SourceDirectory)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetAuthors("Il Harper")
                .SetCopyright("2021 Il Harper")
                .SetTitle("Depth Estimate")
                .SetDescription("Depth Estimate GUI - Windows, Mac, Linux")
                .SetRepositoryType("git")
                .SetRepositoryUrl("https://github.com/depth-estimate-gui/depth-estimate-gui.git")
                .SetRuntime(Runtime)
                // .SetSelfContained(PublishRelease) // dotnet/sdk/issues/10902
                .EnablePublishReadyToRun()
                .EnablePublishTrimmed());
        });

    Target PackProgram => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            Logger.Info("Cleaning output directory.");
            EnsureCleanDirectory(OutputDirectory);

            Logger.Info("Packing program.");
            ForceCopyDirectoryRecursively(
                SourceDirectory / "bin" / Configuration / "net6.0" / Runtime / "publish",
                OutputDirectory);

            EnsureCleanDataDirectory();
        });

    Target PackTools => _ => _
        .DependsOn(PackProgram)
        .Executes(() =>
        {
            Logger.Info("Cleaning tools directory.");
            EnsureCleanDirectory(ToolsDirectory);

            AbsolutePath tempDirectory = TemporaryDirectory / "tools";
            EnsureCleanDirectory(tempDirectory);

            Logger.Info("Ejecting Conda environment.");
            ProcessTasks.StartShell(
                    $"conda create -y -p . python=3.7 pytorch torchvision {(CudaVersion == "cpuonly" ? CudaVersion : "cudatoolkit=" + CudaVersion)} opencv matplotlib timm -c pytorch-lts",
                    ToolsDirectory)
                .AssertZeroExitCode();

            if (Platform is PlatformFamily.Linux or PlatformFamily.OSX)
            {
                Logger.Info("Configuring privileges.");
                ProcessTasks.StartShell(
                        "chmod -R 777 depth-estimate-gui",
                        DistDirectory)
                    .AssertZeroExitCode();
            }

            Logger.Info("Downloading core repositories.");

            AbsolutePath monodepthZip = tempDirectory / "monodepth.zip";
            AbsolutePath midasZip = tempDirectory / "midas.zip";
            AbsolutePath monodepthExtractDir = tempDirectory / "monodepth";
            AbsolutePath midasExtractDir = tempDirectory / "midas";
            AbsolutePath monodepthTargetDir = ToolsDirectory / "monodepth2";
            AbsolutePath midasTargetDir = ToolsDirectory / "midas";

            HttpTasks.HttpDownloadFile(
                "https://github.com/nianticlabs/monodepth2/archive/refs/heads/master.zip",
                monodepthZip);
            CompressionTasks.UncompressZip(monodepthZip, monodepthExtractDir);
            ForceCopyDirectoryRecursively(monodepthExtractDir / "monodepth2-master", monodepthTargetDir);

            HttpTasks.HttpDownloadFile(
                "https://github.com/isl-org/MiDaS/archive/refs/heads/master.zip",
                midasZip);
            CompressionTasks.UncompressZip(midasZip, midasExtractDir);
            ForceCopyDirectoryRecursively(midasExtractDir / "MiDaS-master", midasTargetDir);

            Logger.Success("Core repositories downloaded.");
            Logger.Info("Downloading models.");

            AbsolutePath monodepthModelZip = tempDirectory / "monodepth_model.zip";
            AbsolutePath monodepthModelTarget = monodepthTargetDir / "models";
            AbsolutePath midasModelTarget = midasTargetDir / "weights" / "dpt_large.pt";

            HttpTasks.HttpDownloadFile(
                "https://storage.googleapis.com/niantic-lon-static/research/monodepth2/mono%2Bstereo_1024x320.zip",
                monodepthModelZip);
            CompressionTasks.UncompressZip(
                monodepthModelZip,
                monodepthModelTarget);

            HttpTasks.HttpDownloadFile(
                "https://github.com/intel-isl/DPT/releases/download/1_0/dpt_large-midas-2f21e586.pt",
                midasModelTarget);

            Logger.Success("Model downloaded.");

            Logger.Info("Copying OneKey-Generator scripts.");
            AbsolutePath scriptsDir = RootDirectory / "scripts";
            CopyFileToDirectory(scriptsDir / "monodepth2_1kgen.py", monodepthTargetDir);
            CopyFileToDirectory(scriptsDir / "midas_1kgen.py", midasTargetDir);

            Logger.Info("Plotting colormaps.");
            ProcessTasks.StartProcess(
                    ToolsDirectory / (Platform == PlatformFamily.Windows ? "python.exe" : "python"),
                    "cmapgen.py",
                    scriptsDir)
                .AssertZeroExitCode();
            AbsolutePath cmapDir = OutputDirectory / "data" / "cmap";
            EnsureCleanDirectory(cmapDir);
            ForceCopyDirectoryRecursively(scriptsDir / "cmap", cmapDir);
        });

    Target PackRelease => _ => _
        .DependsOn(PackTools)
        .Executes(() =>
        {
            Logger.Info("Compressing dist.");
            CompressionTasks.CompressZip(
                OutputDirectory,
                DistDirectory / "depth-estimate-gui.zip",
                compressionLevel: CompressionLevel.SmallestSize);
        });
}
