using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MaintainableSelenium.MvcPages.Utils;
using MaintainableSelenium.VisualAssertions.Infrastructure;
using MaintainableSelenium.VisualAssertions.Screenshots;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;
using MaintainableSelenium.VisualAssertions.Web.Services.TestResults.Queries;

namespace MaintainableSelenium.VisualAssertions.Web.Services.TestResults
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
                    return ImageHelpers.CreateImageDiff(patternBitmap, errorBitmap, testResult.Pattern.GetAllBlindRegions());
                }
                case ScreenshotType.Diff:
                {
                    var errorBitmap = testResult.ErrorScreenshot.ToBitmap();
                    return ImageHelpers.CreateImagesXor(patternBitmap, errorBitmap, testResult.Pattern.GetAllBlindRegions());
                }
                default:
                    throw new ArgumentOutOfRangeException("screenshotType", screenshotType, null);
            }
        }

        public TestResultListViewModel GetTestsFromSession(long sessionId, string browserName)
        {
            var testSession = this.testSessionRepository.Get(sessionId);
            var testResults = testSession.TestResults.Where(x => x.BrowserName == browserName).ToList();
            return new TestResultListViewModel()
            {
                TestSessionId = testSession.Id,
                BrowserName = browserName,
                TestResults = testResults.ConvertAll(MapToTestResultListItemDTO)
            };
        }

        private static TestResultListItemDTO MapToTestResultListItemDTO(TestResult x)
        {
            return new TestResultListItemDTO()
            {
                TestResultId = x.Id,
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

    public class TestResultListViewModel
    {
        public List<TestResultListItemDTO> TestResults { get; set; }
        public long TestSessionId { get; set; }
        public string BrowserName { get; set; }
    }

    public class TestResultListItemDTO
    {
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