using Microsoft.AspNetCore.Mvc;
using Tellurium.VisualAssertion.Dashboard.Mvc;
using Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegion;
using Tellurium.VisualAssertion.Dashboard.Services.TestCases;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;

namespace Tellurium.VisualAssertion.Dashboard.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ITestCaseService testCaseService;
        private readonly ICommandDispatcher commandDispatcher;

        public TestCaseController(ITestCaseService testCaseService, ICommandDispatcher commandDispatcher)
        {
            this.testCaseService = testCaseService;
            this.commandDispatcher = commandDispatcher;
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

        public ActionResult GetTestCasePattern(long patternId)
        {
            var result = this.testCaseService.GetTestCasePattern(patternId);
            return PartialView(result);
        }

        public ActionResult GetTestCasePatternImage(long patternId)
        {
            var result = this.testCaseService.GetPatternScreenshot(patternId);
            return ActionResultFactory.ImageResult(result);
        }

        [HttpPost]
        public ActionResult SaveBlindRegions([FromBody]SaveBlindRegionsCommand command)
        {
            this.commandDispatcher.Execute(command);
            return ActionResultFactory.AjaxSuccess();
        }

        public ActionResult GetTestCase(long id)
        {
            var result = this.testCaseService.GetTestCase(id);
            return View("TestCaseWithLayout", result);
        }
    }
}