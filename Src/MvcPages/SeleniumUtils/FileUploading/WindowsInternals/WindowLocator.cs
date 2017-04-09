using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tellurium.MvcPages.SeleniumUtils.FileUploading.WindowsInternals
{
    internal static class WindowLocator
    {
        public static List<ControlHandle> GetWindows(string processName)
        {
            return Process.GetProcesses()
                .Where(x => x.ProcessName.Contains(processName))
                .SelectMany(GetFromProcess)
                .ToList();
        }

        private static List<ControlHandle> GetFromProcess(Process process)
        {
            var resultWindowsList = new List<ControlHandle>();
            WindowsMethods.EnumWindows((hWnd, _) =>
            {
                ControlHandle control = hWnd;
                if (control.IsAppWindow())
                    if (process.Id > 0)
                    {
                        var windowProcessId = 0;
                        WindowsMethods.GetWindowThreadProcessId(control, ref windowProcessId);
                        if (windowProcessId == process.Id)
                            resultWindowsList.Add(control);
                    }
                    else
                    {
                        resultWindowsList.Add(control);
                    }
                return true;
            }, 0);
            return resultWindowsList;
        }
    }
}