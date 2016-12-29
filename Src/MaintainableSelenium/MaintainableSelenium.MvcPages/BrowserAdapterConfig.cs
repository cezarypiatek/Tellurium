using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.WebPages.WebForms;
using MaintainableSelenium.MvcPages.WebPages.WebForms.DefaultInputAdapters;

namespace MaintainableSelenium.MvcPages
{
    public class BrowserAdapterConfig
    {
        public const int DefaultNumberOfSetRetries = 3;

        public BrowserType BrowserType { get; set; }

        public string SeleniumDriversPath { get; set; }

        public string PageUrl { get; set; }

        public string ScreenshotsPath { get; set; }

        public  List<IFormInputAdapter> InputAdapters { get; set; }

        public int NumberOfInputSetRetries { get; set; }

        public BrowserDimensionsConfig BrowserDimensions { get; set; }

        public BrowserCameraConfig BrowserCameraConfig { get; set; }
        
        public AfterFieldValueSet AfterFieldValueSetAction { get; set; }

        public BrowserAdapterConfig()
        {
            InputAdapters = new List<IFormInputAdapter>
            {
                    new TextFormInputAdapter(),
                    new SelectFormInputAdapter(),
                    new CheckboxFormInputAdapter(),
                    new RadioFormInputAdapter(),
                    new HiddenFormInputAdapter()
            };
            NumberOfInputSetRetries = DefaultNumberOfSetRetries;
            AfterFieldValueSetAction = AfterFieldValueSet.Nothing;
        }

        /// <summary>
        /// Create config from mantainableSeleniumConfiguration section in app.config
        /// </summary>
        /// <param name="testExecutionPath">Path to location where test is executed</param>
        public static BrowserAdapterConfig FromAppConfig(string testExecutionPath)
        {
            var config = (MantainableSeleniumConfigurationSection)ConfigurationManager.GetSection("maintainableSeleniumConfiguration");
            var seleniumDriversPath = config.DriversPath;
            if (Path.IsPathRooted(seleniumDriversPath) == false)
            {
                seleniumDriversPath = Path.Combine(testExecutionPath, seleniumDriversPath);
            }
            return new BrowserAdapterConfig
            {
                BrowserType = config.Browser,
                SeleniumDriversPath = seleniumDriversPath,
                ScreenshotsPath = config.ErrorScreenshotsPath,
                PageUrl = config.PageUrl
            };
        }
    }
}