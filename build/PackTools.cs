using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;

partial class Build
{
    Target PackTools => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Logger.Info("Cleaning tools directory.");
            EnsureCleanDirectory(ToolsDirectory);

            AbsolutePath tempDirectory = TemporaryDirectory / "tools";
            EnsureCleanDirectory(tempDirectory);

            Logger.Info("Ejecting Conda environment.");
            ProcessTasks.StartShell(
                    $"{(Platform is (PlatformFamily.Linux or PlatformFamily.OSX) ? "sudo " : "")}conda create -q -y --copy -p . python=3.7 pytorch torchvision {(CudaVersion == "cpuonly" ? CudaVersion : "cudatoolkit=" + CudaVersion)} opencv matplotlib timm -c pytorch-lts -c conda-forge",
                    ToolsDirectory)
                .AssertZeroExitCode();

            // if (Platform is PlatformFamily.Linux or PlatformFamily.OSX)
            //     ProcessTasks.StartShell(
            //             "ln -s ./bin/python ./python",
            //             ToolsDirectory)
            //         .AssertZeroExitCode();

            EnsurePrivilege();

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
            CopyFile(scriptsDir / "monodepth2_1kgen.py", monodepthTargetDir / "1kgen.py", FileExistsPolicy.Overwrite);
            CopyFile(scriptsDir / "midas_1kgen.py", midasTargetDir / "1kgen.py", FileExistsPolicy.Overwrite);

            Logger.Info("Plotting colormaps.");

            string pathValue = Platform is PlatformFamily.Linux or PlatformFamily.OSX
                ? $"{ToolsDirectory / "bin"}:"
                : $"{ToolsDirectory};{ToolsDirectory / "Library" / "bin"};{ToolsDirectory / "Scripts"};";
            pathValue += Environment.GetEnvironmentVariable("PATH");
            IDictionary envOrigin = Environment.GetEnvironmentVariables();
            Dictionary<string, string> env = envOrigin
                .Cast<DictionaryEntry>()
                .ToDictionary(entry => (string)entry.Key, entry => (string)entry.Value);
            if (env.ContainsKey("Path"))
                env["Path"] = pathValue;
            if (env.ContainsKey("PATH"))
                env["PATH"] = pathValue;
            Logger.Info("Using PATH variable: " + pathValue);

            ProcessTasks.StartProcess(
                    ToolsDirectory / (Platform == PlatformFamily.Windows ? "python.exe" : "python"),
                    "cmapgen.py",
                    scriptsDir,
                    env)
                .AssertZeroExitCode();
            AbsolutePath cmapDir = OutputDirectory / "data" / "cmap";
            EnsureCleanDirectory(cmapDir);
            ForceCopyDirectoryRecursively(scriptsDir / "cmap", cmapDir);

            Logger.Info("Cleaning temporary directory.");
            EnsureCleanDirectory(tempDirectory);
            Logger.Info("Cleaning conda cache.");
            ProcessTasks.StartShell(
                    $"{(Platform is (PlatformFamily.Linux or PlatformFamily.OSX) ? "sudo " : "")}conda clean -y -a")
                .AssertZeroExitCode();

            Logger.Info("Compressing dist.");
            // CompressionTasks.CompressZip(
            //     OutputDirectory,
            //     DistDirectory / "depth-estimate-gui-tools.zip",
            //     compressionLevel: CompressionLevel.SmallestSize);
            ProcessTasks.StartShell(
                    $"7z a {DistDirectory / "depth-estimate-gui-tools.7z"} {OutputDirectory} -mx9")
                .AssertZeroExitCode();
        });
}
