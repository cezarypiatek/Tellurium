using System;
using System.Collections.Generic;
using System.Linq;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Queries;
using Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing;
using Tellurium.VisualAssertions.TestRunersAdapters;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class VisualAssertionsService:IDisposable
    {
        private readonly IRepository<Project> projectRepository;
        private readonly ITestRunnerAdapter testRunnerAdapter;
        private readonly ISet<ScreenshotIdentity> takenScreenshots = new HashSet<ScreenshotIdentity>();
        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public string BrowserName { get; set; }

        private ITaskProcessor<Screenshot> ScreenshotProcessor; 

        public VisualAssertionsService(
            IRepository<Project> projectRepository, 
            ITestRunnerAdapter testRunnerAdapter,
            bool processAsynchronously) 
        {
            this.projectRepository = projectRepository;
            this.testRunnerAdapter = testRunnerAdapter;
            InitScreenshotProcessor(processAsynchronously);
        }

        private void InitScreenshotProcessor(bool processAsynchronously)
        {
            var processorType = processAsynchronously ? TaskProcessorType.Async : TaskProcessorType.Sync;
            this.ScreenshotProcessor = TaskProcessorFactory.Create<Screenshot>(processorType, this.CheckScreenshotWithPattern);
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
            ScreenshotProcessor.Post(new Screenshot
            {
                Identity = screenshotIdentity,
                Data = screenshot
            });
        }

        public class Screenshot
        {
            public ScreenshotIdentity Identity { get; set; }
            public byte[] Data { get; set; }
        }

        private void CheckScreenshotWithPattern(Screenshot screenshot)
        {
            byte[] image = screenshot.Data;
            ScreenshotIdentity screenshotIdentity = screenshot.Identity;
            try
            {
                Action finishNotification;
                using (var tx = PersistanceEngine.GetSession().BeginTransaction())
                {
                    var project = this.GetProject(screenshotIdentity.ProjectName);
                    var testCase = GetTestCase(project, screenshotIdentity);
                    var activePattern = testCase.GetActivePatternForBrowser(screenshotIdentity.BrowserName);
                    
                    var newPattern = activePattern == null ? testCase.AddNewPattern(image, screenshotIdentity.BrowserName) : null;
                    var testResult = GetTestResult(image, screenshotIdentity, activePattern, newPattern);

                    var testSession = GetCurrentTestSession(project);
                    testSession.AddTestResult(testResult);
                    finishNotification = () =>
                    {
                        switch (testResult.Status)
                        {
                            case TestResultStatus.Failed:
                                testRunnerAdapter.NotifyAboutTestFail(screenshotIdentity.FullName, testSession, activePattern);
                                break;
                            case TestResultStatus.Passed:
                                testRunnerAdapter.NotifyAboutTestSuccess(screenshotIdentity.FullName, testSession, activePattern);
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

        private static readonly Dictionary<string, long> ProjectNameIdCache = new Dictionary<string, long>();

        private Project GetProject(string projectName)
        {
            if (ProjectNameIdCache.ContainsKey(projectName))
            {
                var projectId = ProjectNameIdCache[projectName];
                return projectRepository.Get(projectId);
            }

            var project = projectRepository.FindOne(new FindProjectByName(projectName));
            if (project == null)
            {
                project = new Project
                {
                    Name = projectName
                };
                projectRepository.Save(project);
            }
            ProjectNameIdCache[projectName] = project.Id;
            return project;
        }

        public void Dispose()
        {
            ScreenshotProcessor?.Dispose();
        }
    }
}