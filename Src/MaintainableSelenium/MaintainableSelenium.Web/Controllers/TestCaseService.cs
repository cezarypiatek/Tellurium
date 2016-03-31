using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Web.Models.Home;
using MaintainableSelenium.Web.Models.TestCase;

namespace MaintainableSelenium.Web.Controllers
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IRepository<TestCase> testCaseRepository;
        private readonly IRepository<BrowserPattern> browserPatternRepository;
        private readonly IRepository<Project> projectRepository;

        public TestCaseService(IRepository<TestCase> testCaseRepository, 
            IRepository<BrowserPattern> browserPatternRepository,
            IRepository<Project> projectRepository )
        {
            this.testCaseRepository = testCaseRepository;
            this.browserPatternRepository = browserPatternRepository;
            this.projectRepository = projectRepository;
        }

        public void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto)
        {
            var testCase = this.testCaseRepository.Get(dto.TestCaseId);
            var browserPattern = testCase.Patterns.First(x => x.Id == dto.BrowserPatternId);
            browserPattern.BlindRegions.Clear();
            browserPattern.BlindRegions.AddRange(dto.LocalBlindRegions);
            UpdateTestCaseHash(browserPattern, testCase);
        }

        public void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto)
        {
            var testCase = this.testCaseRepository.Get(dto.TestCaseId);
            var globalRegionsForBrowser = testCase.TestCaseSet.GlobalBlindRegions.First(x => x.BrowserName == dto.BrowserName);
            globalRegionsForBrowser.BlindRegions = dto.BlindRegions;
            this.testCaseRepository.FindAll()
                .AsParallel()
                .ForAll(tc =>
                {
                    var bp = tc.Patterns.FirstOrDefault(x => x.BrowserName == dto.BrowserName);
                    if (bp != null)
                    {
                        UpdateTestCaseHash(bp, tc);
                    }
                });
        }

        private void UpdateTestCaseHash(BrowserPattern browserPattern, TestCase testCase)
        {
            var blindRegionForBrowser = testCase.TestCaseSet.GlobalBlindRegions.First(x => x.BrowserName == browserPattern.BrowserName);
            browserPattern.PatternScreenshot.Hash = ImageHelpers.ComputeHash(browserPattern.PatternScreenshot.Image, blindRegionForBrowser.BlindRegions, browserPattern.BlindRegions);
        }

        public List<TestCaseListItem> GetAll()
        {
            var testCases = this.testCaseRepository.FindAll();
            return testCases.Select(x => new TestCaseListItem
            {
                TestCaseName = string.Format("{0}\\{1}", x.TestName, x.PatternScreenshotName),
                Browsers = x.Patterns.Select(y => new BrowserPatternShortcut
                {
                    BrowserName = y.BrowserName,
                    PatternId= y.Id
                }).ToList()
            }).ToList();
        }

        public BrowserPatternDTO GetTestCasePattern(long testCaseId, long patternId)
        {
            var testCase = this.testCaseRepository.Get(testCaseId);
            var browserPattern = browserPatternRepository.Get(patternId);
            var blindRegionForBrowser = testCase.TestCaseSet.GlobalBlindRegions.First(x=>x.BrowserName == browserPattern.BrowserName);
            return new BrowserPatternDTO
            {
                GlobalBlindRegions = blindRegionForBrowser.BlindRegions,
                LocalBlindRegions = browserPattern.BlindRegions,
                BrowserName = browserPattern.BrowserName,
                PatternId = browserPattern.Id,
                TestCaseId = testCase.Id
            };
        }

        public byte[] GetPatternScreenshot(long patternId)
        {
            var testCase = this.browserPatternRepository.Get(patternId);
            return testCase.PatternScreenshot.Image;
        }
    }

    public interface ITestCaseService
    {
        void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto);
        void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto);
        List<TestCaseListItem> GetAll();
        BrowserPatternDTO GetTestCasePattern(long testCaseId, long patternId);
        byte[] GetPatternScreenshot(long patternId);
    }
}