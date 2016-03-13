using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            var globalBlindRegions = TestRepository.GetGlobalBlindRegions(testCase.BrowserName);
            return View(new TestCaseDetailsDTO()
            {
                GlobalBlindRegions = globalBlindRegions,
                TestCase = testCase
            });
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
                    var globalBlindRegions = this.TestRepository.GetGlobalBlindRegions(testCase.BrowserName);
                    var blindRegions = testCase.BlindRegions.ToList();
                    blindRegions.AddRange(globalBlindRegions);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    blindRegions = blindRegions.Select(x =>
                    {
                        var xCoef = bitmap2.Width/100.0f;
                        var yCoef = bitmap2.Height/100.0f;
                        return new BlindRegion(x.Left*xCoef, x.Top*yCoef, x.Width*xCoef, x.Height*yCoef);
                    }).ToList();
                    var diff = ImageHelpers.CreateImageDiff(bitmap1, bitmap2, blindRegions);
                    return ImageResult(diff);
                }
                case ScreenshotType.Diff:
                {
                    var globalBlindRegions = this.TestRepository.GetGlobalBlindRegions(testCase.BrowserName);
                    var blindRegions = testCase.BlindRegions.ToList();
                    blindRegions.AddRange(globalBlindRegions);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    blindRegions = blindRegions.Select(x =>
                    {
                        var xCoef = bitmap2.Width / 100.0f;
                        var yCoef = bitmap2.Height / 100.0f;
                        return new BlindRegion(x.Left * xCoef, x.Top * yCoef, x.Width * xCoef, x.Height * yCoef);
                    }).ToList();
                    var xor = ImageHelpers.CreateImagesXor(bitmap1, bitmap2, blindRegions);
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

        [HttpPost]
        public ActionResult SaveLocalBlindspots(SaveLocalBlindRegionsDTO dto)
        {
            this.TestRepository.SaveBlindregions(dto.TestCaseId, dto.LocalBlindRegions);
            return Json(new { success = true });
        }
        
        [HttpPost]
        public ActionResult SaveGlobalBlindspots(SaveGlobalBlindRegionsDTO dto)
        {
            this.TestRepository.SaveGlobalBlindregions(dto.BrowserName, dto.BlindRegions);
            return Json(new { success = true });
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
        public TestCaseInfo TestCase { get; set; }
        public List<BlindRegion> GlobalBlindRegions { get; set; }
    }
}