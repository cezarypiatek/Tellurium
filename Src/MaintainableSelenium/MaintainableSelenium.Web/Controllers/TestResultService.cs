using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestResultService : ITestResultService
    {
        private readonly IRepository<TestResult> testRepository;
        private readonly IGlobalRegionsSource globalRegionsSource;
        private readonly IRepository<TestCase> testCaseRepository;
        private readonly IRepository<TestSession> testSessionRepository;

        public TestResultService(IRepository<TestResult> testRepository, 
            IGlobalRegionsSource globalRegionsSource, 
            IRepository<TestCase> testCaseRepository,
            IRepository<TestSession>  testSessionRepository
        )
        {
            this.testRepository = testRepository;
            this.globalRegionsSource = globalRegionsSource;
            this.testCaseRepository = testCaseRepository;
            this.testSessionRepository = testSessionRepository;
        }

        public TestResult GetTestResult(long testResultId)
        {
            return this.testRepository.Get(testResultId);
        }

        public void MarkAsPattern(long testResultId)
        {
            var testResult = this.testRepository.Get(testResultId);
            testResult.TestPassed = true;
            testResult.TestCase.PatternScreenshot.Image = testResult.ErrorScreenshot.Image;
            testResult.TestCase.PatternScreenshot.Hash = testResult.ErrorScreenshot.Hash;
        }

        public void MarkAllAsPattern(long testSessionId, string browserName)
        {
            var testResults = this.testSessionRepository.Get(testSessionId).TestResults.Where(x=>x.BrowserName == browserName);
            foreach (var testResult in testResults.Where(x => x.TestPassed == false))
            {
                testResult.TestCase.PatternScreenshot.Image = testResult.ErrorScreenshot.Image;
                testResult.TestCase.PatternScreenshot.Hash = testResult.ErrorScreenshot.Hash;
            }
        }

        public Bitmap GetScreenshot(long testId, ScreenshotType screenshotType)
        {
            var testResult = this.testRepository.Get(testId);
            var testCase = this.testCaseRepository.Get(testResult.TestCase.Id);
            var bitmap1 = ImageHelpers.ConvertBytesToBitmap(testCase.PatternScreenshot.Image);

            switch (screenshotType)
            {
                case ScreenshotType.Pattern:
                    return bitmap1;
                case ScreenshotType.Error:
                {
                    var globalBlindRegions = this.globalRegionsSource.GetGlobalBlindRegions(testCase.BrowserName);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    return ImageHelpers.CreateImageDiff(bitmap1, bitmap2, globalBlindRegions, testCase.BlindRegions);
                }
                case ScreenshotType.Diff:
                {
                    var globalBlindRegions = this.globalRegionsSource.GetGlobalBlindRegions(testCase.BrowserName);
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    return ImageHelpers.CreateImagesXor(bitmap1, bitmap2, globalBlindRegions, testCase.BlindRegions);
                }
                default:
                    throw new ArgumentOutOfRangeException("screenshotType", screenshotType, null);
            }
        }

        public List<TestResult> GetTestsFromSession(long sessionId, string browserName)
        {
            return this.testSessionRepository.Get(sessionId).TestResults.Where(x => x.BrowserName == browserName).ToList();
        }


        public List<TestSession> GetTestSessions()
        {
            return this.testSessionRepository.FindAll()
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }
    }

    public interface ITestResultService
    {
        Bitmap GetScreenshot(long testId, ScreenshotType screenshotType);
        TestResult GetTestResult(long testResultId);
        void MarkAsPattern(long testResultId);
        void MarkAllAsPattern(long testSessionId, string browserName);
        List<TestResult> GetTestsFromSession(long sessionId, string browserName);
        List<TestSession> GetTestSessions();
    }

    public enum ScreenshotType
    {
        Pattern = 1,
        Error,
        Diff
    }
}