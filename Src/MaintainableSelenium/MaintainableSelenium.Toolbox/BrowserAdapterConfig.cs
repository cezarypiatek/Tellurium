using System.Collections.Generic;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using MaintainableSelenium.Toolbox.WebPages.WebForms;

namespace MaintainableSelenium.Toolbox
{
    public class BrowserAdapterConfig
    {
        public BrowserType BrowserType { get; set; }

        public string SeleniumDriversPath { get; set; }

        public string ProjectName { get; set; }

        public string ScreenshotCategory { get; set; }

        public string PageUrl { get; set; }

        public  List<IFormInputAdapter> InputAdapters { get; set; }
    }
}