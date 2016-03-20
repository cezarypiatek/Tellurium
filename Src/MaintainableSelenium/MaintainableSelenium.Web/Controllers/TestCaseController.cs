using System.Web.Mvc;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestRepository testRepository;

        public TestCaseController()
        {
            this.testRepository = TestRepositoryFactory.Create();
        }

        public ActionResult GetTestCases()
        {
            var testCases = testRepository.GetTestCases();
            return View(testCases);
        }

        public ActionResult GetTestCaseDetails(string testCaseId)
        {
            var testCase = testRepository.GetTestCase(testCaseId);
            var globalBlindRegions = testRepository.GetGlobalBlindRegions(testCase.BrowserName);
            return View(new TestCaseDetailsDTO
            {
                GlobalBlindRegions = globalBlindRegions,
                TestCase = testCase
            });
        }

        public ActionResult GetTestCasePatternImage(string testCaseId)
        {
            var testCase = this.testRepository.GetTestCase(testCaseId);
            return ActionResultFactory.ImageResult(testCase.PatternScreenshot);
        }

        [HttpPost]
        public ActionResult SaveLocalBlindspots(SaveLocalBlindRegionsDTO dto)
        {
            this.testRepository.SaveLocalBlindregions(dto.TestCaseId, dto.LocalBlindRegions);
            return ActionResultFactory.AjaxSuccess();
        }

        [HttpPost]
        public ActionResult SaveGlobalBlindspots(SaveGlobalBlindRegionsDTO dto)
        {
            this.testRepository.SaveGlobalBlindregions(dto.BrowserName, dto.BlindRegions);
            return ActionResultFactory.AjaxSuccess();
        }
    }
}