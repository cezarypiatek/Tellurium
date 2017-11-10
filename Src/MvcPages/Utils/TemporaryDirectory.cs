using System;
using System.Diagnostics;
using System.IO;

namespace Tellurium.MvcPages.Utils
{
    internal class TemporaryDirectory:IDisposable
    {
        public string Path { get; }

        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public string[] GetFiles()
        {
            return Directory.GetFiles(Path);
        }

        public void Clear()
        {
            Directory.Delete(Path, true);
            Directory.CreateDirectory(Path);
        }
     

        public void Dispose()
        {
            Directory.Delete(Path, true);
        }
    }

    internal static class FileExtensions
    {
        [DebuggerStepThrough]
        public static bool IsFileLocked(string filePath)
        {
            var file = new FileInfo(filePath);
            try
            {
                using(file.Open(FileMode.Open, FileAccess.Read, FileShare.None)){}
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}