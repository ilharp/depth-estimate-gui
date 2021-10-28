using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.EnvironmentInfo;

partial class Build
{
    void ForceCopyDirectoryRecursively(string source, string target) =>
        CopyDirectoryRecursively(
            source,
            target,
            DirectoryExistsPolicy.Merge,
            FileExistsPolicy.OverwriteIfNewer);

    void EnsureCleanDataDirectory()
    {
        EnsureCleanDirectory(OutputDirectory / "data");
        EnsureCleanDirectory(OutputDirectory / "data" / "inputs");
        EnsureCleanDirectory(OutputDirectory / "data" / "outputs");
    }

    void EnsurePrivilege()
    {
        if (Platform is not (PlatformFamily.Linux or PlatformFamily.OSX)) return;

        Logger.Info("Configuring privileges.");
        ProcessTasks.StartShell(
                "sudo chmod -R 777 depth-estimate-gui",
                DistDirectory)
            .AssertZeroExitCode();
    }
}
