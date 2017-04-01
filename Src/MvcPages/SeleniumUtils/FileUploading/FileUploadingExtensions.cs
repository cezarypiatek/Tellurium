using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

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
            if (File.Exists(filePath) == false)
            {
                throw new FileUploadException($"Cannot upload file '{filePath}'. File does not exist.");
            }

            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                using (RunspaceInvoke invoker = new RunspaceInvoke(runspace))
                {
                    var script = ReadFileContentFromEmbededResource("FileUploader.psm1");
                    try
                    {
                        var waspLocation = typeof(Huddled.Wasp.Constants).Assembly.Location;
                        invoker.Invoke($"Import-Module \"{waspLocation}\"");
                        invoker.Invoke(script);
                        invoker.Invoke($"Upload-File -BrowserName {browserName} -FilePath \"{filePath}\"");
                    }
                    catch(Exception ex)
                    {
                        throw new FileUploadException($"Cannot upload file {filePath}. Reason: '{ex.Message}'");
                    }
                }
            }
        }

        private static string ReadFileContentFromEmbededResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
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
