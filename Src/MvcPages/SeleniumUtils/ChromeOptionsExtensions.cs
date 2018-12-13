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

        public static void DisableSpellCheck(this ChromeOptions options)
        {
            options.AddUserProfilePreference("browser.enable_spellchecking", "false");
            options.AddUserProfilePreference("browser.enable_autospellcorrect", "false");
            options.AddUserProfilePreference("spellcheck.use_spelling_service", "");
            options.AddUserProfilePreference("spellcheck.dictionary", "");
            options.AddUserProfilePreference("translate.enabled", "false");
            
        }
    }
}