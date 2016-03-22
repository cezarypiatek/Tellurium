using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Models.Home
{
    public class SaveGlobalBlindRegionsDTO
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }

        public SaveGlobalBlindRegionsDTO()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}