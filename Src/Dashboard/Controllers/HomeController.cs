using Microsoft.AspNetCore.Mvc;
using Tellurium.VisualAssertion.Dashboard.Mvc;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults;

namespace Tellurium.VisualAssertion.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestResultService testResultService;

        public HomeController(ITestResultService testResultService)
        {
            this.testResultService = testResultService;
        }

        // GET: Home
        public ActionResult Index()
        {
            var testSessions = this.testResultService.GetProjectsList();
            return View("ProjectList", testSessions);
        }

        public ActionResult GetTestSessionFromProject(long projectId)
        {
            var testSessions = this.testResultService.GetTestSessionsFromProject(projectId);
            return PartialView("TestSessionList", testSessions);
        }

        public ActionResult GetTestsFromSessionSession(long sessionId, string browserName)
        {
            var tests = this.testResultService.GetTestsFromSession(sessionId, browserName);
            return View("TestResultList", tests);
        }

        public ActionResult GetTestsFromSessionInStatus(long sessionId, string browserName, TestResultStatus status)
        {
            var tests = this.testResultService.GetTestsFromSessionInStatus(sessionId, browserName, status);
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
            this.testResultService.MarkAsPattern(testResultId);
            return this.RedirectToAction("GetTestResult", testResultId);
        }

        [HttpPost]
        public ActionResult MarkAllAsPattern(long testSessionId, string browserName)
        {
            this.testResultService.MarkAllAsPattern(testSessionId, browserName);
            return ActionResultFactory.AjaxSuccess();
        }

        public ActionResult GetTestResultPreview(long testSessionId, long patternId)
        {
            var testResult = this.testResultService.GetTestResultPreview(testSessionId, patternId);
            return this.View("TestResultInfoPreloaded", testResult);
        }
    }
}