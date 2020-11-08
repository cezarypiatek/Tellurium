using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tellurium.MvcPages.SeleniumUtils.FileUploading.WindowsInternals;

namespace Tellurium.MvcPages.SeleniumUtils.FileUploading
{
    public static class FileUploadingExtensions
    {
        public static void UploadFileForCurrentBrowser(string filePath)
        {
            UploadFile(BrowserAdapterContext.Current.BrowserName, filePath);
        }

        public static void UploadFile(string browserName, string filePath)
        {
            var absoluteFilePath = GetAbsoluteExistingPath(filePath);
            var uploadWindow = Robot.GetUploadWindow(browserName);
            uploadWindow.Activate();
            var fileNameInput = uploadWindow.GetControls("Edit").First();
            fileNameInput.Activate();
            WindowsMethods.SendText(fileNameInput, absoluteFilePath);

            var confirmButton = uploadWindow.GetControls("Button").First();
            confirmButton.Activate();
            WindowsMethods.SendClick(confirmButton);
        }

        public static string GetAbsoluteExistingPath(string filePath)
        {
            if (filePath.Contains("\""))
            {
                var absoluteFilesPaths = filePath.Split('"')
                    .Where(x=> string.IsNullOrWhiteSpace(x) == false)
                    .Select(MapToAbsoluteFilePath)
                    .ToList();

                foreach (var path in absoluteFilesPaths)
                {
                    if (File.Exists(path) == false)
                    {
                        throw new FileUploadException($"Cannot upload file '{path}'. File does not exist.");
                    }
                }
                var quotedPaths = absoluteFilesPaths.Select(x => $"\"{x}\"");
                return string.Join(" ", quotedPaths);
            }
            var absoluteFilePath = MapToAbsoluteFilePath(filePath);

            if (File.Exists(absoluteFilePath) == false)
            {
                throw new FileUploadException($"Cannot upload file '{absoluteFilePath}'. File does not exist.");
            }
            return absoluteFilePath;
        }

        private static string MapToAbsoluteFilePath(string filePath)
        {
            return Path.IsPathRooted(filePath) ? filePath : ToAbsolutePath(filePath);
        }

        private static string ToAbsolutePath(string filePath)
        {
			// TODO: This needs to be reworked if we want to target .NET Core (however it will work on .NET 5.0)
            string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (privateBinPath != null)
            {
                foreach (var rootPath in privateBinPath.Split(';'))
                {
                    var absolutePath = Path.Combine(System.AppContext.BaseDirectory, rootPath, filePath);
                    if (File.Exists(absolutePath))
                    {
                        return absolutePath;
                    }
                }
            }
            return Path.Combine(System.AppContext.BaseDirectory, filePath);
        }

        private static string ReadFileContentFromEmbeddedResource(string fileName)
        {
            var assembly = typeof(FileUploadingExtensions).GetTypeInfo().Assembly;
            var currentNamespace = typeof(FileUploadingExtensions).Namespace;
            using (Stream stream = assembly.GetManifestResourceStream($"{currentNamespace}.{fileName}"))
            {
                if (stream == null)
                {
                    throw new ApplicationException("Cannot load file from embeded resource");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
