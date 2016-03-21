
namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly ITestRepository testRepository;
        private readonly ITestCaseRepository testCaseRepository;
        private readonly ITestSessionRepository testSessionRepository;
        private TestSessionInfo testSessionData;
        

        public ScreenshotService(
            ITestRunnerAdapter testRunnerAdapter, 
            ITestRepository testRepository, 
            ITestCaseRepository testCaseRepository,
            ITestSessionRepository testSessionRepository)
        {
            this.testRunnerAdapter = testRunnerAdapter;
            this.testRepository = testRepository;
            this.testCaseRepository = testCaseRepository;
            this.testSessionRepository = testSessionRepository;
        }

        private TestSessionInfo GetCurrentTestSession()
        {
            if (testSessionData == null)
            {
                testSessionData = testRunnerAdapter.GetTestSessionInfo();
                testSessionRepository.Save(testSessionData);
            }
            return testSessionData;
        }

        public void Persist(string screenshotName, string browserName, byte[] screenshot)
        {
            var globalBlindRegions = testCaseRepository.GetGlobalBlindRegions(browserName);
            var testName = testRunnerAdapter.GetCurrentTestName();
            var testCaseInfo = testCaseRepository.Find(testName, screenshotName, browserName);
            if (testCaseInfo == null)
            {
                var newTestCase = new TestCase
                {
                    Id = IdGenerator.GetNewId(),
                    TestName = testName,
                    BrowserName = browserName,
                    PatternScreenshotName = screenshotName,
                    PatternScreenshot = new ScreenshotData()
                    {
                        Image = screenshot,
                        Hash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions)
                    }
                };
                this.testCaseRepository.Save(newTestCase);
            }
            else
            {
                var testSession = GetCurrentTestSession();
                var testResult = new TestResultInfo
                {
                    Id = IdGenerator.GetNewId(),
                    TestCaseId = testCaseInfo.Id,
                    TestName = testName,
                    ScreenshotName = screenshotName,
                    BrowserName = browserName
                };

                var screenshotHash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions, testCaseInfo.BlindRegions);
                if (screenshotHash != testCaseInfo.PatternScreenshot.Hash)
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
                testSession.AddTestResult(testResult);
                testRepository.SaveTestResult(testResult);
            }
        }
    }
}