using System.Configuration;
using MaintainableSelenium.MvcPages.SeleniumUtils;

namespace MaintainableSelenium.MvcPages
{
    public class MantainableSeleniumConfigurationSection: ConfigurationSection
    {
        private const string BrowserKey = "browser";
        private const string DriversKey = "driversPath";
        private const string PageUrlKey = "pageUrl";
        private const string ErrorScreenshotsPathKey = "errorScreenshotsPath";

        [ConfigurationProperty(BrowserKey, DefaultValue = BrowserType.Firefox, IsRequired = false)]
        public BrowserType Browser
        {
            get { return (BrowserType)this[BrowserKey]; }
            set { this[BrowserKey] = value;  }
        }

        [ConfigurationProperty(DriversKey, DefaultValue = "Drivers", IsRequired = false)]
        public string DriversPath
        {
            get { return this[DriversKey] as string; }
            set { this[DriversKey] = value; }
        }

        [ConfigurationProperty(PageUrlKey, IsRequired = true)]
        public string PageUrl
        {
            get { return this[PageUrlKey] as string; }
            set { this[PageUrlKey] = value; }
        }

        [ConfigurationProperty(ErrorScreenshotsPathKey, DefaultValue = null, IsRequired = false)]
        public string ErrorScreenshotsPath
        {
            get { return this[ErrorScreenshotsPathKey] as string; }
            set { this[ErrorScreenshotsPathKey] = value; }
        }
    }
}