using System.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly IRepository<Project> projectRepository;
        private TestSession testSessionData;

        public ScreenshotService(ITestRunnerAdapter testRunnerAdapter, IRepository<Project> projectRepository )
        {
            this.testRunnerAdapter = testRunnerAdapter;
            this.projectRepository = projectRepository;
        }

        private TestSession GetCurrentTestSession(Project project)
        {
            if (testSessionData == null)
            {
                testSessionData = testRunnerAdapter.GetTestSessionInfo();
                project.AddSession(testSessionData);
            }
            return testSessionData;
        }

        public void Persist(string screenshotName, string browserName, byte[] screenshot, string projectName)
        {
            var project = this.GetProject(projectName);
            var testName = testRunnerAdapter.GetCurrentTestName();
            var testCase = GetTestCase(project, screenshotName, testName);
            var browserPattern = testCase.Patterns.FirstOrDefault(x => x.BrowserName == browserName);
            if (browserPattern == null)
            {
                testCase.AddNewPattern(screenshot, browserName);
            }
            else
            {
                var testSession = GetCurrentTestSession(project);
                var testResult = new TestResult
                {
                    Pattern = browserPattern,
                    TestName = testName,
                    ScreenshotName = screenshotName,
                    BrowserName = browserName
                };

                if (browserPattern.MatchTo(screenshot) == false)
                {
                    testResult.TestPassed = false;
                    testResult.ErrorScreenshot = CreateErrorScreenshotData(browserName, screenshot, project, browserPattern);
                }
                else
                {
                    testResult.TestPassed = true;
                }
                testSession.AddTestResult(testResult);
            }
        }

        private static ScreenshotData CreateErrorScreenshotData(string browserName, byte[] screenshot, Project project, BrowserPattern browserPattern)
        {
            var globalBlindRegions = project.TestCaseSet.GetBlindRegionsForBrowser(browserName);
            var errorScreenshot = new ScreenshotData
            {
                Image = screenshot,
                Hash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions, browserPattern.BlindRegions)
            };
            return errorScreenshot;
        }

        private static TestCase GetTestCase(Project project, string screenshotName, string testName)
        {
            var testCase = project.TestCaseSet.TestCases.FirstOrDefault(x => x.PatternScreenshotName == screenshotName && x.TestName == testName);
            if (testCase == null)
            {
                testCase = new TestCase
                {
                    TestName = testName,
                    PatternScreenshotName = screenshotName
                };

                project.TestCaseSet.AddTestCase(testCase);
            }
            return testCase;
        }

        private Project GetProject(string projectName)
        {
            var project = projectRepository.FindOne(new FindProjectByName(projectName));
            if (project == null)
            {
                project = new Project
                {
                    Name = projectName
                };
                projectRepository.Save(project);
            }
            return project;
        }
    }
}