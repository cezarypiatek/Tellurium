using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Firefox;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class FirefoxOptionsExtensions
    {
        public static void EnableHeadless(this FirefoxOptions options)
        {
            options.AddArgument("--headless");
        }

        public static void EnableFileDownloading(this FirefoxProfile profile, string downloadDir,  IReadOnlyList<string> allowedMimeTypes)
        {
            profile.SetPreference("browser.download.folderList", 2);
            profile.SetPreference("browser.download.show_plugins_in_list", false);
            profile.SetPreference("browser.download.pluginOverrideTypes", false);
            profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            profile.SetPreference("browser.download.useDownloadDir", true);
            profile.SetPreference("plugin.disable_full_page_plugin_for_types", "application/pdf");
            profile.SetPreference("browser.download.panel.shown", false);
            profile.SetPreference("browser.download.dir", downloadDir);
            profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            profile.SetPreference("browser.download.manager.closeWhenDone", true);
            profile.SetPreference("browser.download.manager.showAlertOnComplete", false);
            profile.SetPreference("browser.download.manager.useWindow", false);
            profile.SetPreference("services.sync.prefs.sync.browser.download.manager.showWhenStarting", false);
            profile.SetPreference("pdfjs.disabled", true);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", string.Join(";", allowedMimeTypes.Distinct()));
        }
    }
}