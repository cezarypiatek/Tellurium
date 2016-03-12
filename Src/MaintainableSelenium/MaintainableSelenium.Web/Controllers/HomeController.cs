using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using MaintainableSelenium.Toolbox.Screenshots;
using Microsoft.Web.Mvc;

namespace MaintainableSelenium.Web.Controllers
{
    public class HomeController : Controller
    {
        private ActionResult ImageResult(byte[] bytes)
        {
            using (var streak = new MemoryStream())
            {
                var srcImage = ImageHelpers.ConvertBytesToImage(bytes);
                srcImage.Save(streak, ImageFormat.Png);
                return File(streak.ToArray(), "image/png");
            }     
        }

        private readonly ITestRepository TestRepository;

        public HomeController()
        {
            this.TestRepository = new FileTestStorage(@"c:\MaintainableSelenium\screenshots");
        }

        // GET: Home
        public ActionResult Index()
        {
            var testSessions = TestRepository.GetTestSessions();
            return View(testSessions);
        }

        public ActionResult GetTestCases()
        {
            var testCases = TestRepository.GetTestCases();
            return View(testCases);
        }

        public ActionResult GetTestCaseDetails(string testCaseId)
        {
            var testCase = TestRepository.GetTestCase(testCaseId);
            return View(testCase);
        }

        public ActionResult GetTestsFromSessionSession(string sessionId, string browserName)
        {
            var tests = this.TestRepository.GetTestsFromSession(sessionId, browserName);
            return View(tests);
        }

        public ActionResult GetTestResultDetails(string testId)
        {
            var test = this.TestRepository.GetTestResult(testId);
            return View(test);
        }

        public ActionResult GetTestCasePatternImage(string testCaseId)
        {
            var testCase = this.TestRepository.GetTestCase(testCaseId);
            return ImageResult(testCase.PatternScreenshot);
        }

        public ActionResult GetScreenshot(string testId, ScreenshotType screenshotType)
        {
            var testResult = this.TestRepository.GetTestResult(testId);
            var testCase = this.TestRepository.GetTestCase(testResult.TestCaseId);
            var bitmap1 = ImageHelpers.ConvertBytesToBitmap(testCase.PatternScreenshot);

            switch (screenshotType)
            {
                case ScreenshotType.Pattern:
                    return ImageResult(testCase.PatternScreenshot);
                case ScreenshotType.Error:
                {
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    var diff = ImageHelpers.CreateImageDiff(bitmap1, bitmap2, testCase.BlindRegions);
                    return ImageResult(diff);
                }
                case ScreenshotType.Diff:
                {
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    var xor = ImageHelpers.CreateImagesXor(bitmap1, bitmap2, testCase.BlindRegions);
                    return ImageResult(xor);
                }
            default:
                    throw new ArgumentOutOfRangeException("screenshotType", screenshotType, null);
            }
        }

        private ActionResult ImageResult(Bitmap diff)
        {
            using (var ms = new MemoryStream())
            {
                diff.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "imgage/png");
            }
        }

        [HttpPost]
        public ActionResult AddBlindRegion(string testCaseId, BlindRegion blindRegion)
        {
            this.TestRepository.AddBlindRegion(testCaseId, blindRegion);
            return Json(new {success = true});
        }

        [HttpPost]
        public ActionResult MarkAsPattern(string testResultId)
        {
            this.TestRepository.MarkAsPattern(testResultId);
            return this.RedirectToAction(c => c.GetTestResult(testResultId));
        }

        public ActionResult GetTestResult(string testId)
        {
            var test = this.TestRepository.GetTestResult(testId);
            return View("TestResultInfo", test);
        }
    }

    public enum ScreenshotType
    {
        Pattern = 1,
        Error,
        Diff
    }
}