using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Screenshots.Domain;

namespace MaintainableSelenium.Web.Models.Home
{
    public class SaveCategoryBlindRegionsDTO
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }
        public long TestCaseId { get; set; }

        public SaveCategoryBlindRegionsDTO()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }
}