using System.Web.Mvc;
using MaintainableSelenium.Web.Models.Home;
using MaintainableSelenium.Web.Mvc;
using MaintainableSelenium.Web.Services.TestCase;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestCaseService testCaseService;

        public TestCaseController(ITestCaseService testCaseService)
        {
            this.testCaseService = testCaseService;
        }

        public ActionResult Index()
        {
            var projectsList = testCaseService.GetProjectsList();
            return View("ProjectsList", projectsList);
        }

        public ActionResult GetTestCasesFromProject(long projectId)
        {
            var testCases = testCaseService.GetTestCasesFromProject(projectId);
            return PartialView(testCases);
        }

        public ActionResult GetTestCasePattern(long testCaseId, long patternId)
        {
            var result = this.testCaseService.GetTestCasePattern(testCaseId, patternId);
            return PartialView(result);
        }

        public ActionResult GetTestCasePatternImage(long patternId)
        {
            var result = this.testCaseService.GetPatternScreenshot(patternId);
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