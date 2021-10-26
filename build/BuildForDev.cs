using Nuke.Common;
using Nuke.Common.IO;
using static Nuke.Common.IO.FileSystemTasks;

// ReSharper disable InvertIf

partial class Build
{
    Target Dev => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Logger.Info("Cleaning output directory.");
            EnsureCleanDirectory(OutputDirectory);

            Logger.Info("Packing program.");
            ForceCopyDirectoryRecursively(
                SourceDirectory / "bin" / Configuration / "net6.0",
                OutputDirectory);

            EnsureCleanDataDirectory();

            AbsolutePath localToolsTempDirectory = RootDirectory / "temp" / "tools";
            AbsolutePath localCmapTempDirectory = RootDirectory / "temp" / "cmap";

            if (DirectoryExists(localToolsTempDirectory))
            {
                EnsureCleanDirectory(ToolsDirectory);
                ForceCopyDirectoryRecursively(localToolsTempDirectory, ToolsDirectory);
            }

            if (DirectoryExists(localCmapTempDirectory))
            {
                EnsureCleanDirectory(OutputDirectory / "data" / "cmap");
                ForceCopyDirectoryRecursively(localCmapTempDirectory, OutputDirectory / "data" / "cmap");
            }
        });
}
