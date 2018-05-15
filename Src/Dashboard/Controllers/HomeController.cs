using Microsoft.AspNetCore.Mvc;
using Tellurium.VisualAssertion.Dashboard.Mvc;
using Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAllAsPattern;
using Tellurium.VisualAssertion.Dashboard.Services.Commands.MarkAsPattern;
using Tellurium.VisualAssertion.Dashboard.Services.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;

namespace Tellurium.VisualAssertion.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestResultService testResultService;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;

        public HomeController(ITestResultService testResultService, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            this.testResultService = testResultService;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
        }

        // GET: Home
        public ActionResult Index()
        {
            var projects = this.queryDispatcher.Execute<GetProjectListQuery, ProjectListViewModel>(new GetProjectListQuery());
            return View("ProjectList", projects);
        }

        public ActionResult GetTestSessionFromProject(GetTestSessionFromProjectQuery query)
        {
            var testSessions = this.queryDispatcher.Execute<GetTestSessionFromProjectQuery, TestSessionListViewModel>(query);
            return PartialView("TestSessionList", testSessions);
        }

        public ActionResult GetTestsFromSessionSession(long sessionId, string browserName)
        {
            var query = new GetTestsFromSessionQuery()
            {
                SessionId = sessionId,
                BrowserName = browserName,
                TestStatus = TestResultStatusFilter.All
            };
            var tests = this.queryDispatcher.Execute<GetTestsFromSessionQuery,GetTestsFromSessionViewModel>(query);
            return View("TestResultList", tests);
        }

        public ActionResult GetTestsFromSessionInStatus(long sessionId, string browserName, TestResultStatusFilter status)
        {
            var query = new GetTestsFromSessionQuery()
            {
                SessionId = sessionId,
                BrowserName = browserName,
                TestStatus = status
            };
            var tests = this.queryDispatcher.Execute<GetTestsFromSessionQuery,GetTestsFromSessionViewModel>(query);
            return View("TestResultsInStatus", tests);
        }

        public ActionResult GetTestResult(long testId)
        {
            var test = this.testResultService.GetTestResult(testId);
            return View("TestResultInfo", test);
        }

        public ActionResult GetTestResultDetails(long testId)
        {
            var test = this.testResultService.GetTestResultDetails(testId);
            return View("TestResultDetails", test);
        }

        public ActionResult GetScreenshot(long testId, ScreenshotType screenshotType)
        {
            var image = this.testResultService.GetScreenshot(testId, screenshotType);
            return ActionResultFactory.ImageResult(image);
        }

        [HttpPost]
        public ActionResult MarkAsPattern(long testResultId)
        {
            this.commandDispatcher.Execute(new MarkAsPatternCommand(testResultId));
            return this.RedirectToAction("GetTestResult", new { testId= testResultId});
        }

        [HttpPost]
        public ActionResult MarkAllAsPattern(long testSessionId, string browserName)
        {
            this.commandDispatcher.Execute(new MarkAllAsPatternCommand(testSessionId, browserName));
            return ActionResultFactory.AjaxSuccess();
        }

        public ActionResult GetTestResultPreview(long testSessionId, long patternId)
        {
            var testResult = this.testResultService.GetTestResultPreview(testSessionId, patternId);
            return this.View("TestResultInfoPreloaded", testResult);
        }
    }
}