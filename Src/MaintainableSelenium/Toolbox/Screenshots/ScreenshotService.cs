using System.Linq;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly IRepository<Project> projectRepository;

        public ScreenshotService(ITestRunnerAdapter testRunnerAdapter, IRepository<Project> projectRepository )
        {
            this.testRunnerAdapter = testRunnerAdapter;
            this.projectRepository = projectRepository;
        }

        private TestSession GetCurrentTestSession(Project project)
        {
            var sessionData = testRunnerAdapter.GetTestSessionInfo();
            var testSession = project.Sessions.FirstOrDefault(x => x.StartDate == sessionData.StartDate);
            if (testSession == null)
            {
                testSession = new TestSession()
                {
                    StartDate = sessionData.StartDate
                };
                project.AddSession(testSession);
            }
            return testSession;
        }

        public void Persist(string screenshotName, string browserName, byte[] screenshot, string projectName)
        {
            var session = PersistanceEngine.GetSession();
            {
                using (var tx = session.BeginTransaction())
                {
                    var project = this.GetProject(projectName);
                    var testCase = GetTestCase(project, screenshotName);
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
                            ScreenshotName = screenshotName,
                            BrowserName = browserName
                        };

                        if (browserPattern.MatchTo(screenshot) == false)
                        {
                            testResult.TestPassed = false;
                            testResult.ErrorScreenshot = CreateErrorScreenshotData(browserName, screenshot, project,
                                browserPattern);
                        }
                        else
                        {
                            testResult.TestPassed = true;
                        }
                        testSession.AddTestResult(testResult);
                    }
                    session.Flush();
                    tx.Commit();
                }
            }
        }

        private static ScreenshotData CreateErrorScreenshotData(string browserName, byte[] screenshot, Project project, BrowserPattern browserPattern)
        {
            var globalBlindRegions = project.GetBlindRegionsForBrowser(browserName);
            var errorScreenshot = new ScreenshotData
            {
                Image = screenshot,
                Hash = ImageHelpers.ComputeHash(screenshot, globalBlindRegions, browserPattern.BlindRegions)
            };
            return errorScreenshot;
        }

        private static TestCase GetTestCase(Project project, string screenshotName)
        {
            var testCase = project.TestCases.FirstOrDefault(x => x.PatternScreenshotName == screenshotName);
            if (testCase == null)
            {
                testCase = new TestCase
                {
                    PatternScreenshotName = screenshotName
                };

                project.AddTestCase(testCase);
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