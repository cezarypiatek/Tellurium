using System;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Infrastructure.Persistence;
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

        public void Persist(byte[] image, ScreenshotIdentity screenshotIdentity)
        {
            using (var tx = PersistanceEngine.GetSession().BeginTransaction())
            {
                var project = this.GetProject(screenshotIdentity.ProjectName);
                var testCase = GetTestCase(project, screenshotIdentity);
                var browserPattern = testCase.GetActivePatternForBrowser(screenshotIdentity.BrowserName);
                if (browserPattern == null)
                {
                    testCase.AddNewPattern(image, screenshotIdentity.BrowserName);
                }
                else
                {
                    var testSession = GetCurrentTestSession(project);
                    var testResult = new TestResult
                    {
                        Pattern = browserPattern,
                        ScreenshotName = screenshotIdentity.ScreenshotName,
                        Category = screenshotIdentity.Category,
                        BrowserName = screenshotIdentity.BrowserName
                    };

                    if (browserPattern.MatchTo(image) == false)
                    {
                        testResult.TestPassed = false;
                        testResult.ErrorScreenshot = image;
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

        private static TestCase GetTestCase(Project project, ScreenshotIdentity screenshotIdentity)
        {
            var testCaseCategory = project.TestCaseCategories.FirstOrDefault(x=> x.Name == screenshotIdentity.Category);
            if (testCaseCategory == null)
            {
                testCaseCategory = project.AddTestCaseCategory(screenshotIdentity.Category);
            }
            var testCase = testCaseCategory.TestCases.FirstOrDefault(x => x.PatternScreenshotName == screenshotIdentity.ScreenshotName);
            if (testCase == null)
            {
                testCase = testCaseCategory.AddTestCase(screenshotIdentity.ScreenshotName);
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