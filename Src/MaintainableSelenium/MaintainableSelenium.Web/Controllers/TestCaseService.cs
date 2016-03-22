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
        private readonly IGlobalRegionsSource globalRegionsSource;

        public TestCaseService(IRepository<TestCase> testCaseRepository, IGlobalRegionsSource globalRegionsSource)
        {
            this.testCaseRepository = testCaseRepository;
            this.globalRegionsSource = globalRegionsSource;
        }

        public void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto)
        {
            var testCase = this.testCaseRepository.Get(dto.TestCaseId);
            testCase.BlindRegions.Clear();
            testCase.BlindRegions.AddRange(dto.LocalBlindRegions);
            UpdateTestCaseHash(testCase);
        }

        public void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto)
        {
           this.globalRegionsSource.SaveGlobalBlindRegions(dto.BrowserName, dto.BlindRegions);
            this.testCaseRepository.FindAll()
                .Where(x => x.BrowserName == dto.BrowserName)
                .AsParallel()
                .ForAll(UpdateTestCaseHash);
        }

        private void UpdateTestCaseHash(TestCase testCase)
        {
            var global = this.globalRegionsSource.GetGlobalBlindRegions(testCase.BrowserName);
            testCase.PatternScreenshot.Hash = ImageHelpers.ComputeHash(testCase.PatternScreenshot.Image, global, testCase.BlindRegions);
        }

        public List<ExtendedTestCaseInfo> GetAll()
        {
            var testCases = this.testCaseRepository.FindAll();
            return testCases.GroupBy(x => x.PatternScreenshotName).Select(x => new ExtendedTestCaseInfo
            {
                TestCaseName = x.Key,
                Browsers = x.Select(y => new TestCaseShortcut
                {
                    BrowserName = y.BrowserName,
                    TestCaseId = y.Id
                }).ToList()
            }).ToList();
        }

        public TestCaseDetailsDTO GetDetails(long testCaseId)
        {
            var testCase = testCaseRepository.Get(testCaseId);
            var globalBlindRegions = globalRegionsSource.GetGlobalBlindRegions(testCase.BrowserName);
            return new TestCaseDetailsDTO
            {
                GlobalBlindRegions = globalBlindRegions,
                TestCase = testCase
            };
        }

        public byte[] GetPatternScreenshot(long testCaseId)
        {
            var testCase = this.testCaseRepository.Get(testCaseId);
            return testCase.PatternScreenshot.Image;
        }
    }

    public interface ITestCaseService
    {
        void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto);
        void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto);
        List<ExtendedTestCaseInfo> GetAll();
        TestCaseDetailsDTO GetDetails(long testCaseId);
        byte[] GetPatternScreenshot(long testCaseId);
    }
}