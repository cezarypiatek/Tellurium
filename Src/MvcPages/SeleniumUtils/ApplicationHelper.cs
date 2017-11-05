using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Tellurium.MvcPages.SeleniumUtils
{
    internal static class ApplicationHelper
    {
        public static string GetOperaBinaryLocation()
        {
            foreach (var app in GetAppInstalationInfo("Opera"))
            {
                if (string.IsNullOrWhiteSpace(app.Directory))
                {
                    continue;
                }

                var operExeLocations = Directory.GetFiles(app.Directory, "opera.exe", SearchOption.AllDirectories);
                if (operExeLocations.Length > 0)
                {
                    return operExeLocations.First();
                }
            }
            return null;
        }

        static IEnumerable<AppInstallationInfo> GetAppInstalationInfo(string expectedName)
        {
            foreach (var registryView in new []{RegistryView.Registry32, RegistryView.Registry64})
            {
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
                var parentKey = baseKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                if (parentKey == null)
                {
                    continue;
                }

                foreach (var subKeyName in parentKey.GetSubKeyNames())
                {
                    var regKey =  parentKey.OpenSubKey(subKeyName);
                    var appName = regKey?.GetValue("DisplayName")?.ToString();
                    if (appName != null && appName.Contains(expectedName))
                    {
                        yield return new AppInstallationInfo
                        {
                            Name = appName,
                            Directory = regKey.GetValue("InstallLocation")?.ToString()
                        };
                    }
                }
            }
        }
        
        [DebuggerDisplay("{Name} - {Directory}")]
        class AppInstallationInfo
        {
            public string Name { get; set; }
            public string Directory { get; set; }
        }
    }
}