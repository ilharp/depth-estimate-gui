using System;
using System.IO;
using ReactiveUI;

namespace DepthEstimateGui.Core
{
    public partial class Graphic : ReactiveObject, IDisposable
    {
        public Guid Id = Guid.NewGuid();

        public Graphic(string path)
        {
            SourceName = path;
            _sourceExt = Path.GetExtension(path);
            InputPath = GraphicStorage.GetInputFilePath(InputName);
            File.Copy(path, InputPath);
        }

        public string SourceName { get; }

        private readonly string _sourceExt;

        private string InputName => Id + _sourceExt;

        public string InputPath;

        public void Dispose()
        {
            _processComplete.Dispose();
        }
    }

    public static class GraphicStorage
    {
        private static readonly string InputDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "inputs");

        private static readonly string OutputDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "outputs");

        public static void EnsureDirectoryExists()
        {
            Directory.CreateDirectory(InputDir);
            Directory.CreateDirectory(OutputDir);
        }

        public static string GetInputFilePath(string name)
        {
            EnsureDirectoryExists();
            return Path.Combine(InputDir, name);
        }

        public static string GetOutputFilePath(string name)
        {
            EnsureDirectoryExists();
            return Path.Combine(OutputDir, name);
        }
    }
}
