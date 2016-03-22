using System.Web.Mvc;
using MaintainableSelenium.Web.Mvc;
using Microsoft.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestResultService testResultService;

        public HomeController()
        {
        }

        // GET: Home
        public ActionResult Index()
        {
            var testSessions = this.testResultService.GetTestSessions();
            return View(testSessions);
        }

        public ActionResult GetTestsFromSessionSession(long sessionId, string browserName)
        {
            var tests = this.testResultService.GetTestsFromSession(sessionId, browserName);
            return View(tests);
        }

        public ActionResult GetTestResult(long testId)
        {
            var test = this.testResultService.GetTestResult(testId);
            return View("TestResultInfo", test);
        }

        public ActionResult GetTestResultDetails(long testId)
        {
            var test = this.testResultService.GetTestResult(testId);
            return View(test);
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
            return this.RedirectToAction(c => c.GetTestResult(testResultId));
        }

        [HttpPost]
        public ActionResult MarkAllAsPattern(long testSessionId, string browserName)
        {
            this.testResultService.MarkAllAsPattern(testSessionId, browserName);
            return ActionResultFactory.AjaxSuccess();
        }
    }
}