using System.Linq;
using System.Threading;
using Tellurium.MvcPages.SeleniumUtils.FileUploading.WindowsInternals;


namespace Tellurium.MvcPages.SeleniumUtils.FileUploading
{
    internal static class Robot
    {
        internal static ControlHandle GetUploadWindow(string browserName)
        {
            var leftTries = 30;
            do
            {
                var uploadWindow = TryGetUploadWindow(browserName);
                if (uploadWindow != null)
                {
                    return uploadWindow;
                }
                Thread.Sleep(1000);
            } while (leftTries-- > 0);
            throw new FileUploadException("Cannot find upload window");
        }

        internal static ControlHandle TryGetUploadWindow(string browserName)
        {
            var processName = GetBrowserProcessName(browserName);
            var browserWindow = WindowLocator.GetWindows(processName);
            return browserWindow.SelectMany(w => w.GetChildrenWindows()).FirstOrDefault();
        }

        private static string GetBrowserProcessName(string browserName)
        {
            switch (browserName)
            {
                case "Firefox":
                    return "firefox";
                case "Chrome":
                    return "chrome";
                case "InternetExplorer":
                    return "iexplore";
                case "Opera":
                    return "opera";
                case "Edge":
                    return "MicrosoftEdge";
                default:
                    throw new FileUploadException("Not supported browser");
            }
        }
    }
}