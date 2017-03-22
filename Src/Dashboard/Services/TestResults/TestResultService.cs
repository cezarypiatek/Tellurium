using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tellurium.MvcPages.Utils;
using Tellurium.VisualAssertion.Dashboard.Mvc.Utils;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults.Queries;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestResults
{
    public class TestResultService : ITestResultService
    {
        private readonly IRepository<TestResult> testRepository;
        private readonly IRepository<TestSession> testSessionRepository;
        private readonly IRepository<Project> projectRepository;
        private readonly ISessionContext sessionContext;

        public TestResultService(IRepository<TestResult> testRepository, 
            IRepository<TestSession>  testSessionRepository,
            IRepository<Project>  projectRepository,
            ISessionContext sessionContext
        )
        {
            this.testRepository = testRepository;
            this.testSessionRepository = testSessionRepository;
            this.projectRepository = projectRepository;
            this.sessionContext = sessionContext;
        }

        public TestResultListItemDTO GetTestResult(long testResultId)
        {
            var testResult = this.testRepository.Get(testResultId);
            return MapToTestResultListItemDTO(testResult);
        }

        public void MarkAsPattern(long testResultId)
        {
            using (var tx = sessionContext.Session.BeginTransaction())
            {
                var testResult = this.testRepository.Get(testResultId);
                testResult.MarkAsPattern();
                tx.Commit();
            }
        }

        public void MarkAllAsPattern(long testSessionId, string browserName)
        {
            using (var tx = sessionContext.Session.BeginTransaction())
            {
                var failedTestResultsQuery = new FindFailedTestFromSessionForBrowser(testSessionId, browserName);
                var failedTestResults = this.testRepository.FindAll(failedTestResultsQuery);
                foreach (var testResult in failedTestResults)
                {
                    testResult.MarkAsPattern();
                }
                tx.Commit();
            }
        }

        public Bitmap GetScreenshot(long testId, ScreenshotType screenshotType)
        {
            var testResult = this.testRepository.Get(testId);
            var patternBitmap = testResult.Pattern.PatternScreenshot.Image.ToBitmap();

            switch (screenshotType)
            {
                case ScreenshotType.Pattern:
                    return patternBitmap;
                case ScreenshotType.Error:
                {

                    var errorBitmap = testResult.ErrorScreenshot.ToBitmap();
                    return ImageHelpers.CreateImageDiff(patternBitmap, errorBitmap, testResult.BlindRegionsSnapshot.AsReadonly());
                }
                case ScreenshotType.Diff:
                {
                    var errorBitmap = testResult.ErrorScreenshot.ToBitmap();
                    return ImageHelpers.CreateImagesXor(patternBitmap, errorBitmap, testResult.BlindRegionsSnapshot.AsReadonly());
                }
                default:
                    throw new ArgumentOutOfRangeException("screenshotType", screenshotType, null);
            }
        }

        public TestResultListViewModel GetTestsFromSession(long sessionId, string browserName)
        {
            var query = new FindTestResultsFromSession(sessionId, browserName, TestResultStatus.All);
            var testResults = this.testRepository.FindAll(query);
            var failedCount = testResults.Count(x => x.TestPassed == false);
            return new TestResultListViewModel()
            {
                AllCount = testResults.Count,
                FailedCount = failedCount,
                PassedCount = testResults.Count-failedCount,
                TestSessionId = sessionId,
                BrowserName = browserName,
                TestResults = testResults.ConvertAll(MapToTestResultListItemDTO)
            };
        }

        private static TestResultListItemDTO MapToTestResultListItemDTO(TestResult x)
        {
            return new TestResultListItemDTO()
            {
                TestResultId = x.Id,
                TestCaseId = x.Pattern.TestCase.Id,
                TestPassed = x.TestPassed,
                ScreenshotName = string.Format("{0} \\ {1}", x.Category, x.ScreenshotName),
                CanShowMarkAsPattern = x.TestPassed == false && x.Pattern.IsActive,
            };
        }


        public TestSessionListViewModel GetTestSessionsFromProject(long projectId)
        {
            var query = new FindAllSessionFromProject(projectId);
            var testSessions = this.testSessionRepository.FindAll(query);
            return new TestSessionListViewModel
            {
                TestSessions = testSessions.Select(x=> new TestSessionListItemDTO
                {
                    SessionId = x.Id,
                    StartDate = x.StartDate.ToString("g"),
                    Browsers = x.Browsers.OrderBy(b=>b).ToList()
                }).ToList()
            };
        }

        public ProjectListViewModel GetProjectsList()
        {
            var projects = projectRepository.GetAll();
            return new ProjectListViewModel()
            {
                Projects = projects.ConvertAll(x=> new ProjectListItemDTO
                {
                    ProjectName = x.Name,
                    ProjectId = x.Id
                })
            };
        }

        public TestResultDetailsViewModel GetTestResultDetails(long testResultId)
        {
            var testResult = this.testRepository.Get(testResultId);
            return new TestResultDetailsViewModel()
            {
                TestPassed = testResult.TestPassed,
                TestResultId = testResult.Id
            };
        }

        public TestResultWithPreview GetTestResultPreview(long testSessionId, long patternId)
        {
            var query = new FindTestResultForPatternInSession(testSessionId, patternId);
            var testResult = this.testRepository.FindOne(query);
            return new TestResultWithPreview()
            {
                ListItem = MapToTestResultListItemDTO(testResult),
                Preview = this.GetTestResultDetails(testResult.Id)
            };
        }

        public TestResultsInStatusViewModel GetTestsFromSessionInStatus(long sessionId, string browserName, TestResultStatus status)
        {
            var query = new FindTestResultsFromSession(sessionId, browserName, status);
            var testResults = this.testRepository.FindAll(query);
            return new TestResultsInStatusViewModel()
            {
                TestResults = testResults.ConvertAll(MapToTestResultListItemDTO)
            };
        }
    }

    public interface ITestResultService
    {
        Bitmap GetScreenshot(long testId, ScreenshotType screenshotType);
        TestResultListItemDTO GetTestResult(long testResultId);
        void MarkAsPattern(long testResultId);
        void MarkAllAsPattern(long testSessionId, string browserName);
        TestResultListViewModel GetTestsFromSession(long sessionId, string browserName);
        TestSessionListViewModel GetTestSessionsFromProject(long projectId);
        ProjectListViewModel GetProjectsList();
        TestResultDetailsViewModel GetTestResultDetails(long testResultId);
        TestResultWithPreview GetTestResultPreview(long testSessionId, long patternId);
        TestResultsInStatusViewModel GetTestsFromSessionInStatus(long sessionId, string browserName, TestResultStatus status);
    }

    public enum ScreenshotType
    {
        Pattern = 1,
        Error,
        Diff
    }

    public class TestResultDetailsViewModel
    {
        public long TestResultId { get; set; }
        public bool TestPassed { get; set; }
    }

    public enum TestResultStatus
    {
        All,
        Passed,
        Failed
    }

    public class TestResultListViewModel
    {
        public List<TestResultListItemDTO> TestResults { get; set; }
        public long TestSessionId { get; set; }
        public string BrowserName { get; set; }
        public int AllCount { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class TestResultsInStatusViewModel
    {
        public List<TestResultListItemDTO> TestResults { get; set; }
    }

    public class TestResultWithPreview
    {
        public TestResultListItemDTO ListItem { get; set; }
        public TestResultDetailsViewModel Preview { get; set; }
    }

    public class TestResultListItemDTO
    {
        public long TestCaseId { get; set; }
        public long TestResultId { get; set; }
        public bool TestPassed { get; set; }
        public string ScreenshotName { get; set; }
        public bool CanShowMarkAsPattern { get; set; }
    }

    public class ProjectListViewModel
    {
        public List<ProjectListItemDTO> Projects { get; set; }
    }

    public class ProjectListItemDTO
    {
        public string ProjectName { get; set; }
        public long ProjectId { get; set; }
    }


    public class TestSessionListViewModel
    {
        public List<TestSessionListItemDTO> TestSessions { get; set; }
    }

    public class TestSessionListItemDTO
    {
        public string StartDate { get; set; }
        public long  SessionId { get; set; }
        public List<string> Browsers { get; set; }
    }
}