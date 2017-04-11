using System.Configuration;
using Tellurium.MvcPages.SeleniumUtils;

namespace Tellurium.MvcPages.Configuration
{
    public class TelluriumConfigurationSection: ConfigurationSection
    {
        private const string BrowserKey = "browser";
        private const string DriversKey = "driversPath";
        private const string PageUrlKey = "pageUrl";
        private const string ErrorScreenshotsPathKey = "errorScreenshotsPath";
        private const string AnimationsDisabledKey = "animationsDisabled";
        private const string MeasureEndpointCoverageKey = "measureEndpointCoverage";
        private const string UseRemoteDriverKey = "useRemoteDriver";
        private const string SeleniumServerUrlKey = "seleniumServerUrl";

        [ConfigurationProperty(BrowserKey, DefaultValue = BrowserType.Firefox, IsRequired = false)]
        public BrowserType Browser
        {
            get { return (BrowserType)this[BrowserKey]; }
            set { this[BrowserKey] = value;  }
        }

        [ConfigurationProperty(DriversKey, DefaultValue = "", IsRequired = false)]
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

        [ConfigurationProperty(AnimationsDisabledKey, DefaultValue = false, IsRequired = false)]
        public bool AnimationsDisabled
        {
            get { return (bool)this[AnimationsDisabledKey]; }
            set { this[AnimationsDisabledKey] = value; }
        }

        [ConfigurationProperty(MeasureEndpointCoverageKey, DefaultValue = false, IsRequired = false)]
        public bool MeasureEndpointCoverage
        {
            get { return (bool)this[MeasureEndpointCoverageKey]; }
            set { this[MeasureEndpointCoverageKey] = value; }
        }

        [ConfigurationProperty(UseRemoteDriverKey, DefaultValue = false, IsRequired = false)]
        public bool UseRemoteDriver
        {
            get { return (bool)this[UseRemoteDriverKey]; }
            set { this[UseRemoteDriverKey] = value; }
        }

        [ConfigurationProperty(SeleniumServerUrlKey, DefaultValue = null, IsRequired = false)]
        public string SeleniumServerUrl
        {
            get { return this[SeleniumServerUrlKey] as string; }
            set { this[SeleniumServerUrlKey] = value; }
        }
    }
}