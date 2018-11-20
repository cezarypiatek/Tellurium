using System;
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

        public Action<string> WriteOutput { get; set; }

        public List<string> AllowedMimeTypes { get; set; }

        public IPageReloadDetector PageReloadDetector { get; set; }

        internal string DownloadDirPath { get; set; }

        public BrowserAdapterConfig()
        {
            InputAdapters = new List<IFormInputAdapter>
            {
                    new TextFormInputAdapter(),
                    new SelectFormInputAdapter(),
                    new CheckboxFormInputAdapter(),
                    new RadioFormInputAdapter(),
                    new FileFormInputAdapter(),
                    new HiddenFormInputAdapter()
            };
            NumberOfInputSetRetries = DefaultNumberOfSetRetries;
            AfterFieldValueSetAction = AfterFieldValueSet.Nothing;
            AllowedMimeTypes = new List<string>
            {
                "text/plain",
                "text/html",
                "text/xml",
                "text/csv",
                "text/richtext",
                "application/soap+xml",
                "application/octet-stream",
                "application/rtf",
                "application/pdf",
                "application/zip",
                "application/x-zip",
                "application/x-zip-compressed",
                "application/csv",
                "application/download",
                "image/gif",
                "image/tiff",
                "image/png",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "application/vnd.ms-word.document.macroEnabled.12",
                "application/vnd.ms-word.template.macroEnabled.12",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                "application/vnd.ms-excel.sheet.macroEnabled.12",
                "application/vnd.ms-excel.template.macroEnabled.12",
                "application/vnd.ms-excel.addin.macroEnabled.12",
                "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
                "application/vnd.ms-powerpoint",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "application/vnd.openxmlformats-officedocument.presentationml.template",
                "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                "application/vnd.ms-powerpoint.addin.macroEnabled.12",
                "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
                "application/vnd.ms-powerpoint.template.macroEnabled.12",
                "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"
            };
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
    }
}