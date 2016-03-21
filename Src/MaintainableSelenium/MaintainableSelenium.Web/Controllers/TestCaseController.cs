using System.Web.Mvc;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestCaseRepository testCaseRepository;

        public TestCaseController()
        {
            this.testCaseRepository = TestRepositoryFactory.Create() as ITestCaseRepository;
        }

        public ActionResult GetTestCases()
        {
            var testCases = testCaseRepository.GetTestCases();
            return View(testCases);
        }

        public ActionResult GetTestCaseDetails(string testCaseId)
        {
            var testCase = testCaseRepository.Get(testCaseId);
            var globalBlindRegions = testCaseRepository.GetGlobalBlindRegions(testCase.BrowserName);
            return View(new TestCaseDetailsDTO
            {
                GlobalBlindRegions = globalBlindRegions,
                TestCase = testCase
            });
        }

        public ActionResult GetTestCasePatternImage(string testCaseId)
        {
            var testCase = this.testCaseRepository.Get(testCaseId);
            return ActionResultFactory.ImageResult(testCase.PatternScreenshot.Image);
        }

        [HttpPost]
        public ActionResult SaveLocalBlindspots(SaveLocalBlindRegionsDTO dto)
        {
            this.testCaseRepository.SaveLocalBlindregions(dto.TestCaseId, dto.LocalBlindRegions);
            return ActionResultFactory.AjaxSuccess();
        }

        [HttpPost]
        public ActionResult SaveGlobalBlindspots(SaveGlobalBlindRegionsDTO dto)
        {
            this.testCaseRepository.SaveGlobalBlindregions(dto.BrowserName, dto.BlindRegions);
            return ActionResultFactory.AjaxSuccess();
        }
    }
}