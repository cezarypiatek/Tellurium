using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly ITestRepository testRepository;
        private readonly TestSessionInfo TestSessionData;

        public ScreenshotService(ITestRunnerAdapter testRunnerAdapter, ITestRepository testRepository)
        {
            this.testRunnerAdapter = testRunnerAdapter;
            this.testRepository = testRepository;
            this.TestSessionData = testRunnerAdapter.GetTestSessionInfo();
        }

        public void Persist(string screenshotName, string browserName, byte[] screenshot)
        {
            var testName = testRunnerAdapter.GetCurrentTestName();
            var testCaseInfo = testRepository.GetTestCaseInfo(testName, screenshotName, browserName);
            if (testCaseInfo == null)
            {
                var newTestCase = new TestCaseInfo
                {
                    Id = IdGenerator.GetNewId(),
                    TestName = testName,
                    BrowserName = browserName,
                    PatternScreenshotName = screenshotName,
                    PatternScreenshot = screenshot,
                    PatternScreenhotHash = ComputeHash(screenshot)
                };
                this.testRepository.SaveTestCaseInfo(newTestCase);
            }
            else
            {
                var testResult = new TestResultInfo
                {
                    Id = IdGenerator.GetNewId(),
                    TestCaseId = testCaseInfo.Id,
                    TestSession = TestSessionData ,
                    TestName = testName,
                    ScreenshotName = screenshotName,
                    BrowserName = browserName
                };

                var screenshotHash = ComputeHash(screenshot, testCaseInfo.BlindRegions);
                if (screenshotHash != testCaseInfo.PatternScreenhotHash)
                {
                    testResult.TestPassed = false;
                    testResult.ErrorScreenshot = new ScreenshotData
                    {
                        Image = screenshot,
                        Hash = screenshotHash
                    };
                }
                else
                {
                    testResult.TestPassed = true;
                }
                testRepository.SaveTestResult(testResult);
            }
        }

        private string ComputeHash(byte[] screenshot, List<BlindRegion> blindRegions=null)
        {
            var image = ImageHelpers.ConvertBytesToImage(screenshot);
            if (blindRegions != null)
            {
                ImageHelpers.MarkBlindRegions(image, blindRegions);
            }
            
            var imageBytes = ImageHelpers.ConvertImageToBytes(image);
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(imageBytes)).Replace("-","");
            }
        }
    }
}