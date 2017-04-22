using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Queries;
using Tellurium.VisualAssertions.TestRunersAdapters;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertions.TestRunersAdapters.Providers;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class VisualAssertionsService 
    {
        private readonly IRepository<Project> projectRepository;
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly ISet<ScreenshotIdentity> takenScreenshots = new HashSet<ScreenshotIdentity>();
        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public string BrowserName { get; set; }

        public VisualAssertionsService(IRepository<Project> projectRepository, ITestRunnerAdapter testRunnerAdapter)
        {
            this.projectRepository = projectRepository;
            this.testRunnerAdapter = testRunnerAdapter;
        }

        private TestSession GetCurrentTestSession(Project project)
        {
            if (project.Sessions == null)
            {
                throw new ApplicationException("Sessions cannot be null");
            }
            var currentStartDate = TestSessionContext.Current.StartDate;
            var testSession = project.Sessions.FirstOrDefault(x => x.StartDate == currentStartDate);
            if (testSession == null)
            {
                testSession = new TestSession
                {
                    StartDate = currentStartDate
                };
                project.AddSession(testSession);
            }
            return testSession;
        }

        public void CheckViewWithPattern(IBrowserCamera browserCamera, string viewName)
        {
            var screenshotIdentity = new ScreenshotIdentity(ProjectName, BrowserName, ScreenshotCategory, viewName);
            if (takenScreenshots.Contains(screenshotIdentity))
            {
                throw new DuplicatedScreenshotInSession(screenshotIdentity);
            }
            var screenshot = browserCamera.TakeScreenshot();
            takenScreenshots.Add(screenshotIdentity);
            CheckScreenshotWithPattern(screenshot, screenshotIdentity);
        }

        private void CheckScreenshotWithPattern(byte[] image, ScreenshotIdentity screenshotIdentity)
        {
            try
            {
                Action finishNotification;
                using (var tx = PersistanceEngine.GetSession().BeginTransaction())
                {
                    var project = this.GetProject(screenshotIdentity.ProjectName);
                    var testCase = GetTestCase(project, screenshotIdentity);
                    var browserPattern = testCase.GetActivePatternForBrowser(screenshotIdentity.BrowserName);
                    var testSession = GetCurrentTestSession(project);
                    var newPattern = browserPattern == null ? testCase.AddNewPattern(image, screenshotIdentity.BrowserName) : null;
                    var testResult = GetTestResult(image, screenshotIdentity, browserPattern, newPattern);
                    testSession.AddTestResult(testResult);
                    finishNotification = () =>
                    {
                        switch (testResult.Status)
                        {
                            case TestResultStatus.Failed:
                                testRunnerAdapter.NotifyAboutTestFail(screenshotIdentity.FullName, testSession, browserPattern);
                                break;
                            case TestResultStatus.Passed:
                                testRunnerAdapter.NotifyAboutTestSuccess(screenshotIdentity.FullName, testSession, browserPattern);
                                break;
                            case TestResultStatus.NewPattern:
                                testRunnerAdapter.NotifyAboutTestSuccess(screenshotIdentity.FullName, testSession, newPattern);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    };

                    tx.Commit();
                }
                finishNotification.Invoke();
            }
            catch (Exception ex)
            {
                testRunnerAdapter.NotifyAboutError(ex);
            }
        }

        private TestResult GetTestResult(byte[] image, ScreenshotIdentity screenshotIdentity, BrowserPattern browserPattern, BrowserPattern newPattern)
        {
            var testResult = new TestResult
            {
                Pattern = newPattern ?? browserPattern,
                ScreenshotName = screenshotIdentity.ScreenshotName,
                Category = screenshotIdentity.Category,
                BrowserName = screenshotIdentity.BrowserName
            };

            if (newPattern != null)
            {
                testResult.Status = TestResultStatus.NewPattern;
            }
            else if (browserPattern.MatchTo(image))
            {
                testResult.Status = TestResultStatus.Passed;
            }
            else
            {
                testResult.Status = TestResultStatus.Failed;
                testResult.ErrorScreenshot = image;
                testResult.BlindRegionsSnapshot = browserPattern.GetCopyOfAllBlindRegions();
            }
            return testResult;
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