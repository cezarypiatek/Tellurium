using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Models.Home
{
    public class TestCaseDetailsDTO
    {
        public Toolbox.Screenshots.TestCase TestCase { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
    }
}