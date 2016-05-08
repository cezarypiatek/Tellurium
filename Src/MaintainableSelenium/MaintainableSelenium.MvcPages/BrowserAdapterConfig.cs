using System.Collections.Generic;
using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.WebPages.WebForms;
using MaintainableSelenium.MvcPages.WebPages.WebForms.DefaultInputAdapters;

namespace MaintainableSelenium.MvcPages
{
    public class BrowserAdapterConfig
    {
        public BrowserType BrowserType { get; set; }

        public string SeleniumDriversPath { get; set; }

        public string PageUrl { get; set; }

        public  List<IFormInputAdapter> InputAdapters { get; set; }

        public BrowserDimensionsConfig BrowserDimensions { get; set; }

        public BrowserCameraConfig BrowserCameraConfig { get; set; }

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
        }
    }
}