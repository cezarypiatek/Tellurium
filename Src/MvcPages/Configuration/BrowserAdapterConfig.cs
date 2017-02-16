using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.WebPages.WebForms;
using Tellurium.MvcPages.WebPages.WebForms.DefaultInputAdapters;

namespace Tellurium.MvcPages.Configuration
{
    public class BrowserAdapterConfig
    {
        public const int DefaultNumberOfSetRetries = 3;

        public BrowserType BrowserType { get; set; }

        public string SeleniumDriversPath { get; set; }

        public string PageUrl { get; set; }

        public string ErrorScreenshotsPath { get; set; }

        public  List<IFormInputAdapter> InputAdapters { get; set; }

        public int NumberOfInputSetRetries { get; set; }

        public BrowserDimensionsConfig BrowserDimensions { get; set; }

        public BrowserCameraConfig BrowserCameraConfig { get; set; }
        
        public AfterFieldValueSet AfterFieldValueSetAction { get; set; }

        public bool AnimationsDisabled { get; set; }

        public bool MeasureEndpointCoverage { get; set; }

        public List<string> AvailableEndpoints { get; set; }

        public List<Assembly> AvailableEndpointsAssemblies { get; set; }

        public string ErrorReportOutputDir { get; set; }

        public bool UseRemoteDriver { get; set; }

        public string SeleniumServerUrl { get; set; }

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
            var config = (TelluriumConfigurationSection)ConfigurationManager.GetSection("telluriumConfiguration");
            var seleniumDriversPath = config.DriversPath;
            if (Path.IsPathRooted(seleniumDriversPath) == false)
            {
                seleniumDriversPath = Path.Combine(testExecutionPath, seleniumDriversPath);
            }
            return new BrowserAdapterConfig
            {
                BrowserType = config.Browser,
                SeleniumDriversPath = seleniumDriversPath,
                ErrorScreenshotsPath = config.ErrorScreenshotsPath,
                PageUrl = config.PageUrl,
                AnimationsDisabled = config.AnimationsDisabled,
                MeasureEndpointCoverage = config.MeasureEndpointCoverage,
                UseRemoteDriver =  config.UseRemoteDriver,
                SeleniumServerUrl = config.SeleniumServerUrl
            };
        }

        public IEnumerable<string> GetAvailableEndpoints()
        {
            if (this.AvailableEndpoints?.Count > 0)
            {
                foreach (var endpoint in AvailableEndpoints.AsReadOnly())
                {
                    yield return endpoint;
                }
            }

            if (this.AvailableEndpointsAssemblies?.Count > 0 )
            {
                foreach (var endpoint in GetEndpintsFromAssemblies())
                {
                    yield return MvcEndpointsHelper.NormalizeEndpointAddress(endpoint);
                }    
            }
        }

        private IEnumerable<string> GetEndpintsFromAssemblies()
        {
            foreach (var endpointsAssembly in AvailableEndpointsAssemblies)
            {
                foreach (var endpoint in MvcEndpointsHelper.GetAvailablePagesFromAssembly(endpointsAssembly))
                {
                    yield return MvcEndpointsHelper.NormalizeEndpointAddress(endpoint);
                }
            }
        }
    }
}