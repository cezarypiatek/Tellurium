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
     

        public TestResultService(IRepository<TestResult> testRepository)
        {
            this.testRepository = testRepository;
        }

        public TestResultListItemDTO GetTestResult(long testResultId)
        {
            var testResult = this.testRepository.Get(testResultId);
            return MapToTestResultListItemDTO(testResult);
        }

        public Bitmap GetScreenshot(long testId, ScreenshotType screenshotType)
        {
            var query = new FindErrorScreenshot(testId);
            var testResult = this.testRepository.FindOne(query);
            var patternBitmap = testResult.Pattern.PatternScreenshot.Image.ToBitmap();
            var errorBitmap = testResult.ErrorScreenshot.ToBitmap();
            switch (screenshotType)
            {
                case ScreenshotType.Error:
                    return ImageHelpers.CreateImageDiff(patternBitmap, errorBitmap, testResult.BlindRegionsSnapshot.AsReadonly());
                case ScreenshotType.Diff:
                    return ImageHelpers.CreateImagesXor(patternBitmap, errorBitmap, testResult.BlindRegionsSnapshot.AsReadonly());
                default:
                    throw new ArgumentOutOfRangeException(nameof(screenshotType), screenshotType, null);
            }
        }

     

        public TestResultDetailsViewModel GetTestResultDetails(long testResultId)
        {
            var query = new FindTestResultDetails(testResultId);
            var testResult = this.testRepository.FindOne(query);
            return new TestResultDetailsViewModel()
            {
                TestPassed = testResult.Status == TestResultStatus.Passed,
                TestFailed = testResult.Status == TestResultStatus.Failed,
                TestResultId = testResult.Id,
                PatternId = testResult.Pattern.Id,
                TestCaseId= testResult.Pattern.TestCase.Id
            };
        }

       
    }

    public interface ITestResultService
    {
        Bitmap GetScreenshot(long testId, ScreenshotType screenshotType);
        TestResultListItemDTO GetTestResult(long testResultId);
        TestResultDetailsViewModel GetTestResultDetails(long testResultId);
        
    }

    public enum ScreenshotType
    {
        Error=2,
        Diff
    }

    public class TestResultDetailsViewModel
    {
        public long TestResultId { get; set; }
        public bool TestPassed { get; set; }
        public bool TestFailed { get; set; }
        public long PatternId { get; set; }
        public long TestCaseId { get; set; }
    }

    public class TestResultsInStatusViewModel
    {
        public List<TestResultListItemDTO> TestResults { get; set; }
    }

  

    public class TestResultListItemDTO
    {
        public long TestCaseId { get; set; }
        public long TestResultId { get; set; }
        public bool TestPassed { get; set; }
        public string ScreenshotName { get; set; }
        public bool CanShowMarkAsPattern { get; set; }
        public bool TestFailed { get; set; }
        public long TestPatternId { get; set; }
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