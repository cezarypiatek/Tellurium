using System.Web.Mvc;
using MaintainableSelenium.Web.Models.Home;
using MaintainableSelenium.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestCaseService testCaseService;

        public ActionResult GetTestCases()
        {
            var testCases = testCaseService.GetAll();
            return View(testCases);
        }

        public ActionResult GetTestCaseDetails(long testCaseId)
        {
            var result = this.testCaseService.GetDetails(testCaseId);
            return View(result);
        }

        public ActionResult GetTestCasePatternImage(long testCaseId)
        {
            var result = this.testCaseService.GetPatternScreenshot(testCaseId);
            return ActionResultFactory.ImageResult(result);
        }

        [HttpPost]
        public ActionResult SaveLocalBlindspots(SaveLocalBlindRegionsDTO dto)
        {
            this.testCaseService.SaveLocalBlindregions(dto);
            return ActionResultFactory.AjaxSuccess();
        }

        [HttpPost]
        public ActionResult SaveGlobalBlindspots(SaveGlobalBlindRegionsDTO dto)
        {
            this.testCaseService.SaveGlobalBlindregions(dto);
            return ActionResultFactory.AjaxSuccess();
        }
    }
}