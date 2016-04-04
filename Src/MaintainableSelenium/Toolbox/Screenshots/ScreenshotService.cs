using System;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using MaintainableSelenium.Toolbox.Screenshots.Queries;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotService
    {
        private readonly IRepository<Project> projectRepository;
        private static readonly DateTime SessionStartDate = DateTime.Now;

        public ScreenshotService(IRepository<Project> projectRepository )
        {
            this.projectRepository = projectRepository;
        }

        private TestSession GetCurrentTestSession(Project project)
        {
            var testSession = project.Sessions.FirstOrDefault(x => x.StartDate == SessionStartDate);
            if (testSession == null)
            {
                testSession = new TestSession
                {
                    StartDate = SessionStartDate
                };
                project.AddSession(testSession);
            }
            return testSession;
        }

        public void Persist(string screenshotName, string browserName, byte[] screenshot, string projectName)
        {
            using (var tx = PersistanceEngine.GetSession().BeginTransaction())
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
                tx.Commit();
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