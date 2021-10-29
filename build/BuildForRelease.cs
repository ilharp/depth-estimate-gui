using System.IO.Compression;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
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
                .SetDescription("Depth Estimate GUI.")
                .SetRepositoryType("git")
                .SetRepositoryUrl("https://github.com/depth-estimate-gui/depth-estimate-gui.git")
                .SetRuntime(Runtime)
                .EnableSelfContained()
                .EnablePublishReadyToRun());
                // .EnablePublishTrimmed());
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

    Target PackRelease => _ => _
        .DependsOn(PackProgram)
        .Executes(() =>
        {
            Logger.Info("Compressing dist.");
            CompressionTasks.CompressZip(
                OutputDirectory,
                DistDirectory / "depth-estimate-gui.zip",
                compressionLevel: CompressionLevel.SmallestSize);
        });
}
