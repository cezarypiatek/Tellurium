using OpenQA.Selenium.Chrome;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public static class ChromeOptionsExtensions
    {
        public static void EnableHeadless(this ChromeOptions options)
        {
            options.AddArguments("headless");
            options.AddArgument("disable-gpu");
        } 
        
        public static void EnableFileDownloading(this ChromeOptions options, string downloadDir)
        {
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            options.AddUserProfilePreference("download.prompt_for_download", "false");
            options.AddUserProfilePreference("download.default_directory", downloadDir);
            options.AddUserProfilePreference("plugins.plugins_disabled", new[]{"Chrome PDF Viewer"});
        }
    }
}