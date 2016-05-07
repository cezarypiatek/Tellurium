using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Screenshots.Lens;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using MaintainableSelenium.Toolbox.WebPages.WebForms;
using MaintainableSelenium.Toolbox.WebPages.WebForms.DefaultInputAdapters;

namespace MaintainableSelenium.Toolbox
{
    public class BrowserAdapterConfig
    {
        public BrowserType BrowserType { get; set; }

        public string SeleniumDriversPath { get; set; }

        public string ProjectName { get; set; }

        public string ScreenshotCategory { get; set; }

        public LensType Lens { get; set; }

        public string PageUrl { get; set; }

        public  List<IFormInputAdapter> InputAdapters { get; set; }

        public BrowserDimensionsConfig BrowserDimensions { get; set; }

        public BrowserAdapterConfig()
        {
            Lens = LensType.Regular;
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