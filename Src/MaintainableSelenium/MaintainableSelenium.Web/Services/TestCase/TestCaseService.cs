using System.Collections.Generic;
using System.Linq;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using MaintainableSelenium.Toolbox.Screenshots.Queries;
using MaintainableSelenium.Web.Models.Home;
using MaintainableSelenium.Web.Models.TestCase;
using MaintainableSelenium.Web.Services.TestCase.Queries;
using MaintainableSelenium.Web.Services.TestResult;

namespace MaintainableSelenium.Web.Services.TestCase
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IRepository<Toolbox.Screenshots.Domain.TestCase> testCaseRepository;
        private readonly IRepository<BrowserPattern> browserPatternRepository;
        private readonly IRepository<Project> projectRepository;
        private readonly ISessionContext sessionContext;

        public TestCaseService(IRepository<Toolbox.Screenshots.Domain.TestCase> testCaseRepository, 
            IRepository<BrowserPattern> browserPatternRepository,
            IRepository<Project> projectRepository, ISessionContext sessionContext)
        {
            this.testCaseRepository = testCaseRepository;
            this.browserPatternRepository = browserPatternRepository;
            this.projectRepository = projectRepository;
            this.sessionContext = sessionContext;
        }

        public void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto)
        {
            using (var tx = sessionContext.Session.BeginTransaction())
            {
                var testCase = this.testCaseRepository.Get(dto.TestCaseId);
                var browserPattern = testCase.Patterns.First(x => x.Id == dto.BrowserPatternId);
                browserPattern.ReplaceBlindregionsSet(dto.LocalBlindRegions);
                var blindRegionForBrowser = testCase.Project.GlobalBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == browserPattern.BrowserName)?? new BlindRegionForBrowser();
                UpdateTestCaseHash(browserPattern, blindRegionForBrowser.BlindRegions);
                tx.Commit();
            }
        }

        public void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto)
        {
            using (var tx = sessionContext.Session.BeginTransaction())
            {
                var testCase = this.testCaseRepository.Get(dto.TestCaseId);
                var globalRegionsForBrowser = GetGlobalRegionsForBrowser(dto, testCase);
                globalRegionsForBrowser.ReplaceBlindRegionsSet(dto.BlindRegions);
                var browserPatterns = this.browserPatternRepository.FindAll(new FindPatternsForBrowserInProject(testCase.Project.Id,dto.BrowserName));
                browserPatterns.AsParallel().ForAll(bp =>
                {
                    UpdateTestCaseHash(bp, globalRegionsForBrowser.BlindRegions);
                }); 
                tx.Commit();
            }
        }

        private static BlindRegionForBrowser GetGlobalRegionsForBrowser(SaveGlobalBlindRegionsDTO dto, Toolbox.Screenshots.Domain.TestCase testCase)
        {
            var globalRegionsForBrowser = testCase.Project.GlobalBlindRegionsForBrowsers.FirstOrDefault(x => x.BrowserName == dto.BrowserName);
            if (globalRegionsForBrowser == null)
            {
                globalRegionsForBrowser = new BlindRegionForBrowser
                {
                    BrowserName = dto.BrowserName
                };
                testCase.Project.AddGlobalBlindRegions(globalRegionsForBrowser);
            }
            return globalRegionsForBrowser;
        }

        private void UpdateTestCaseHash(BrowserPattern browserPattern, IList<BlindRegion> globalBlindRegions)
        {
            browserPattern.PatternScreenshot.Hash = ImageHelpers.ComputeHash(browserPattern.PatternScreenshot.Image, globalBlindRegions, browserPattern.BlindRegions);
        }

        public List<TestCaseListItem> GetTestCasesFromProject(long projectId)
        {
            var testCases = this.testCaseRepository.FindAll(new FindTestCasesFromProject(projectId));
            return testCases.Select(x => new TestCaseListItem
            {
                TestCaseId = x.Id,
                TestCaseName = x.PatternScreenshotName,
                Browsers = x.Patterns.Select(y => new BrowserPatternShortcut
                {
                    BrowserName = y.BrowserName,
                    PatternId= y.Id,
                }).ToList()
            }).ToList();
        }

        public BrowserPatternDTO GetTestCasePattern(long testCaseId, long patternId)
        {
            var testCase = this.testCaseRepository.Get(testCaseId);
            var browserPattern = browserPatternRepository.Get(patternId);
            var blindRegionForBrowser = testCase.Project.GlobalBlindRegionsForBrowsers.FirstOrDefault(x=>x.BrowserName == browserPattern.BrowserName) ?? new BlindRegionForBrowser();
            return new BrowserPatternDTO
            {
                GlobalBlindRegions = blindRegionForBrowser.BlindRegions.ToList(),
                LocalBlindRegions = browserPattern.BlindRegions.ToList(),
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

        public ProjectListViewModel GetProjectsList()
        {
            var projects = projectRepository.GetAll();
            return new ProjectListViewModel()
            {
                Projects = projects.ConvertAll(x => new ProjectListItemDTO
                {
                    ProjectName = x.Name,
                    ProjectId = x.Id
                })
            };
        }
    }

    public interface ITestCaseService
    {
        void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto);
        void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto);
        List<TestCaseListItem> GetTestCasesFromProject(long projectId);
        BrowserPatternDTO GetTestCasePattern(long testCaseId, long patternId);
        byte[] GetPatternScreenshot(long patternId);
        ProjectListViewModel GetProjectsList();
    }
}