using Nuke.Common.IO;
using static Nuke.Common.IO.FileSystemTasks;

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
}
