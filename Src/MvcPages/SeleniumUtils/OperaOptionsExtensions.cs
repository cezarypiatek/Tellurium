using OpenQA.Selenium.Opera;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class OperaOptionsExtensions
    {
        public static void EnableHeadless(this OperaOptions options)
        {
            options.AddArguments("headless");
            options.AddArgument("disable-gpu");
        } 
        
        public static void EnableFileDownloading(this OperaOptions options, string downloadDir)
        {
            options.AddLocalStatePreference("profile.default_content_settings.popups", 0);
            options.AddLocalStatePreference("download.prompt_for_download", "false");
            options.AddLocalStatePreference("download.default_directory", downloadDir);
            options.AddLocalStatePreference("plugins.plugins_disabled", new[]{"Chrome PDF Viewer"});
        }
    }
}