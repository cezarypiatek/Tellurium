using Microsoft.AspNetCore.Mvc;
using Tellurium.VisualAssertion.Dashboard.Models.Home;
using Tellurium.VisualAssertion.Dashboard.Mvc;
using Tellurium.VisualAssertion.Dashboard.Services.TestCase;

namespace Tellurium.VisualAssertion.Dashboard.Controllers
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
        
        
        public ActionResult GetTestCaseCategories(long projectId)
        {
            var categoriesList = testCaseService.GetTestCaseCategories(projectId);
            return PartialView("CategoriesList", categoriesList);
        }

        public ActionResult GetTestCasesFromCategory(long categoryId)
        {
            var testCases = testCaseService.GetTestCasesFromCategory(categoryId);
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
        public ActionResult SaveLocalBlindspots([FromBody]SaveLocalBlindRegionsDTO dto)
        {
            this.testCaseService.SaveLocalBlindregions(dto);
            return ActionResultFactory.AjaxSuccess();
        }
        
        [HttpPost]
        public ActionResult SaveCategoryBlindspots([FromBody]SaveCategoryBlindRegionsDTO dto)
        {
            this.testCaseService.SaveCategoryBlindregions(dto);
            return ActionResultFactory.AjaxSuccess();
        }

        [HttpPost]
        public ActionResult SaveGlobalBlindspots([FromBody]SaveGlobalBlindRegionsDTO dto)
        {
            this.testCaseService.SaveGlobalBlindregions(dto);
            return ActionResultFactory.AjaxSuccess();
        }

        public ActionResult GetTestCase(long id)
        {
            var result = this.testCaseService.GetTestCase(id);
            return View("TestCaseWithLayout", result);
        }
    }
}