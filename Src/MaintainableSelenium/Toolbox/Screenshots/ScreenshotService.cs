
namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly IRepository<TestCase> testCaseRepository;
        private readonly IRepository<TestSession> testSessionRepository;
        private readonly IGlobalRegionsSource globalRegionsSource;
        private TestSession testSessionData;

        public ScreenshotService(
            ITestRunnerAdapter testRunnerAdapter,
            IRepository<TestCase> testCaseRepository,
            IRepository<TestSession> testSessionRepository,
            IGlobalRegionsSource globalRegionsSource)
        {
            this.testRunnerAdapter = testRunnerAdapter;
            this.testCaseRepository = testCaseRepository;
            this.testSessionRepository = testSessionRepository;
            this.globalRegionsSource = globalRegionsSource;
        }

        private TestSession GetCurrentTestSession()
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
            var globalBlindRegions = globalRegionsSource.GetGlobalBlindRegions(browserName);
            var testName = testRunnerAdapter.GetCurrentTestName();
            var testCaseInfo = FindTestCase(screenshotName, browserName, testName);
            if (testCaseInfo == null)
            {
                var newTestCase = new TestCase
                {
                    TestName = testName,
                    BrowserName = browserName,
                    PatternScreenshotName = screenshotName,
                    PatternScreenshot = new ScreenshotData
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
                var testResult = new TestResult
                {
                    TestCase = testCaseInfo,
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
            }
        }

        private TestCase FindTestCase(string screenshotName, string browserName, string testName)
        {
            var query = FindTestCaseForBrowser.Create(testName, screenshotName, browserName);
            var testCaseInfo = testCaseRepository.FindOne(query);
            return testCaseInfo;
        }
    }
}