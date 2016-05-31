using System.Collections.Generic;
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
    }
}