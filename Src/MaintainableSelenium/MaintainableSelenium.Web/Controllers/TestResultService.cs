using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MaintainableSelenium.Toolbox.Screenshots;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestResultService : ITestResultService
    {
        private readonly IRepository<TestResult> testRepository;
        private readonly IRepository<TestSession> testSessionRepository;
        private readonly IRepository<Project> projectRepository;

        public TestResultService(IRepository<TestResult> testRepository, 
            IRepository<TestSession>  testSessionRepository,
            IRepository<Project>  projectRepository
        )
        {
            this.testRepository = testRepository;
            this.testSessionRepository = testSessionRepository;
            this.projectRepository = projectRepository;
        }

        public TestResultListItemDTO GetTestResult(long testResultId)
        {
            var testResult = this.testRepository.Get(testResultId);
            return MapToTestResultListItemDTO(testResult);
        }

        public void MarkAsPattern(long testResultId)
        {
            TestResult testResult = this.testRepository.Get(testResultId);
            testResult.TestPassed = true;
            ReplacePattern(testResult);
        }

        private static void ReplacePattern(TestResult testResult)
        {
            testResult.Pattern.PatternScreenshot.Image = testResult.ErrorScreenshot.Image;
            testResult.Pattern.PatternScreenshot.Hash = testResult.ErrorScreenshot.Hash;
        }

        public void MarkAllAsPattern(long testSessionId, string browserName)
        {
            var testResults = this.testSessionRepository.Get(testSessionId).TestResults.Where(x=>x.BrowserName == browserName);
            foreach (var testResult in testResults.Where(x => x.TestPassed == false))
            {
                ReplacePattern(testResult);
            }
        }

        public Bitmap GetScreenshot(long testId, ScreenshotType screenshotType)
        {
            var testResult = this.testRepository.Get(testId);
            var blindRegionForBrowser = testResult.TestSession.Project.TestCaseSet.GlobalBlindRegions.First(x => x.BrowserName == testResult.BrowserName);
            var bitmap1 = ImageHelpers.ConvertBytesToBitmap(testResult.Pattern.PatternScreenshot.Image);

            switch (screenshotType)
            {
                case ScreenshotType.Pattern:
                    return bitmap1;
                case ScreenshotType.Error:
                {
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    return ImageHelpers.CreateImageDiff(bitmap1, bitmap2, blindRegionForBrowser.BlindRegions, testResult.Pattern.BlindRegions);
                }
                case ScreenshotType.Diff:
                {
                    var bitmap2 = ImageHelpers.ConvertBytesToBitmap(testResult.ErrorScreenshot.Image);
                    return ImageHelpers.CreateImagesXor(bitmap1, bitmap2, blindRegionForBrowser.BlindRegions, testResult.Pattern.BlindRegions);
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
                TestResults = testResults.ConvertAll(x=> MapToTestResultListItemDTO(x))
            };
        }

        private static TestResultListItemDTO MapToTestResultListItemDTO(TestResult x)
        {
            return new TestResultListItemDTO()
            {
                TestResultId = x.Id,
                TestPassed = x.TestPassed,
                TestName = x.TestName,
                ScreenshotName = x.ScreenshotName
            };
        }


        public TestSessionListViewModel GetTestSessionsFromProject(long projectId)
        {
            var testSessions = this.projectRepository.Get(projectId).Sessions;
            return new TestSessionListViewModel
            {
                TestSessions = testSessions.ConvertAll(x=> new TestSessionListItemDTO
                {
                    SessionId = x.Id,
                    StartDate = x.StartDate.ToShortTimeString(),
                    Browsers = x.Browsers.ToList()
                })
            };
        }

        public ProjectListViewModel GetProjectsList()
        {
            var projects = projectRepository.GetAll();
            return new ProjectListViewModel()
            {
                Projects = projects.ConvertAll(x=> new ProjectListItemDTO()
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
        public string TestName { get; set; }
        public string ScreenshotName { get; set; }
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