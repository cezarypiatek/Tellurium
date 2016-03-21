using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Web.Mvc;
using Microsoft.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITestRepository testRepository;
        private readonly ITestCaseRepository testCaseRepository;
        private readonly ITestSessionRepository testSessionRepository;

        public HomeController()
        {
            var repo = TestRepositoryFactory.Create();
            this.testRepository = repo;
            this.testCaseRepository = repo as ITestCaseRepository;
            this.testSessionRepository = repo as ITestSessionRepository;
        }

        // GET: Home
        public ActionResult Index()
        {
            var testSessions = testSessionRepository.GetTestSessions();
            return View(testSessions);
        }

        public ActionResult GetTestsFromSessionSession(string sessionId, string browserName)
        {
            var tests = this.testRepository.GetTestsFromSession(sessionId, browserName);
            return View(tests);
        }

        public ActionResult GetTestResult(string testId)
        {
            var test = this.testRepository.GetTestResult(testId);
            return View("TestResultInfo", test);
        }

        public ActionResult GetTestResultDetails(string testId)
        {
            var test = this.testRepository.GetTestResult(testId);
            return View(test);
        }

        public ActionResult GetScreenshot(string testId, ScreenshotType screenshotType)
        {
            var testResult = this.testRepository.GetTestResult(testId);
            var testCase = this.testCaseRepository.Get(testResult.TestCaseId);
            var bitmap1 = ImageHelpers.ConvertBytesToBitmap(testCase.PatternScreenshot.Image);
           
            switch (screenshotType)
            {
                case ScreenshotType.Pattern:
                    return ActionResultFactory.ImageResult(bitmap1);
                case ScreenshotType.Error:
                {
                    var globalBlindRegions = this.testCaseRepository.GetGlobalBlindRegions(testCase.BrowserName);
                    var blindRegions = testCase.BlindRegions.ToList();
                    blindRegions.AddRange(globalBlindRegions);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    var diff = ImageHelpers.CreateImageDiff(bitmap1, bitmap2, blindRegions);
                    return ActionResultFactory.ImageResult(diff);
                }
                case ScreenshotType.Diff:
                {
                    var globalBlindRegions = this.testCaseRepository.GetGlobalBlindRegions(testCase.BrowserName);
                    var blindRegions = testCase.BlindRegions.ToList();
                    blindRegions.AddRange(globalBlindRegions);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    var xor = ImageHelpers.CreateImagesXor(bitmap1, bitmap2, blindRegions);
                    return ActionResultFactory.ImageResult(xor);
                }
            default:
                    throw new ArgumentOutOfRangeException("screenshotType", screenshotType, null);
            }
        }

        [HttpPost]
        public ActionResult MarkAsPattern(string testResultId)
        {
            this.testRepository.MarkAsPattern(testResultId);
            return this.RedirectToAction(c => c.GetTestResult(testResultId));
        }

        [HttpPost]
        public ActionResult MarkAllAsPattern(string testSessionId, string browserName)
        {
            testRepository.MarkAllAsPattern(testSessionId, browserName);
            return ActionResultFactory.AjaxSuccess();
        }
    }

    public enum ScreenshotType
    {
        Pattern = 1,
        Error,
        Diff
    }

    public class SaveLocalBlindRegionsDTO
    {
        public string TestCaseId { get; set; }
        public List<BlindRegion> LocalBlindRegions { get; set; }

        public SaveLocalBlindRegionsDTO()
        {
            LocalBlindRegions = new List<BlindRegion>();
        }
    }
    
    public class SaveGlobalBlindRegionsDTO
    {
        public string BrowserName { get; set; }
        public List<BlindRegion> BlindRegions { get; set; }

        public SaveGlobalBlindRegionsDTO()
        {
            BlindRegions = new List<BlindRegion>();
        }
    }

    public class TestCaseDetailsDTO
    {
        public TestCase TestCase { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
    }
}