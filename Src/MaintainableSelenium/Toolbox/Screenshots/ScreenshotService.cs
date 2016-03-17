using System;
using System.Collections.Generic;
using System.Linq;
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
            var globalBlindRegions = testRepository.GetGlobalBlindRegions(browserName);
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
                    PatternScreenhotHash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions)
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
                var blindRegions = testCaseInfo.BlindRegions.ToList();
                blindRegions.AddRange(globalBlindRegions);
                var screenshotHash = ImageHelpers.ComputeHash(screenshot, blindRegions);
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

      
    }
}