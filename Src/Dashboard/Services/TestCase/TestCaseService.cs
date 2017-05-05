using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertion.Dashboard.Models;
using Tellurium.VisualAssertion.Dashboard.Models.Home;
using Tellurium.VisualAssertion.Dashboard.Models.TestCase;
using Tellurium.VisualAssertion.Dashboard.Services.TestCase.Queries;
using Tellurium.VisualAssertion.Dashboard.Services.TestResults;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Queries;

namespace Tellurium.VisualAssertion.Dashboard.Services.TestCase
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IRepository<Tellurium.VisualAssertions.Screenshots.Domain.TestCase> testCaseRepository;
        private readonly IRepository<BrowserPattern> browserPatternRepository;
        private readonly IRepository<Project> projectRepository;
        private readonly ISessionContext sessionContext;
        private readonly IRepository<TestCaseCategory> testCaseCategoryRepository;

        public TestCaseService(IRepository<Tellurium.VisualAssertions.Screenshots.Domain.TestCase> testCaseRepository, 
            IRepository<BrowserPattern> browserPatternRepository,
            IRepository<Project> projectRepository, 
            ISessionContext sessionContext, 
            IRepository<TestCaseCategory> testCaseCategoryRepository)
        {
            this.testCaseRepository = testCaseRepository;
            this.browserPatternRepository = browserPatternRepository;
            this.projectRepository = projectRepository;
            this.sessionContext = sessionContext;
            this.testCaseCategoryRepository = testCaseCategoryRepository;
        }

        public void SaveBlindRegions(SaveBlindRegionsDTO dto)
        {
            using (var tx = sessionContext.Session.BeginTransaction())
            {
                SaveLocalBlindregions(dto.Local);
                SaveCategoryBlindregions(dto.Category);
                SaveGlobalBlindregions(dto.Global);
                tx.Commit();
            }
        }

        private void SaveLocalBlindregions(SaveLocalBlindRegionsDTO dto)
        {
            if (dto == null)
            {
                return;
            }
            var browserPattern = this.browserPatternRepository.Get(dto.BrowserPatternId);
            browserPattern.ReplaceLocalBlindRegionsSet(dto.LocalBlindRegions);
        }

        private void SaveCategoryBlindregions(SaveCategoryBlindRegionsDTO dto)
        {
            if (dto == null)
            {
                return;
            }
            var testCase = this.testCaseRepository.Get(dto.TestCaseId);
            var categoryRegionsForBrowser = GetGlobalBlindRegionsForBrowser(testCase.Category, dto.BrowserName);
            categoryRegionsForBrowser.ReplaceBlindRegionsSet(dto.BlindRegions);
            var browserPatterQUery = new FindPatternsForBrowserInCategory(testCase.Category.Id, dto.BrowserName);
            var browserPatterns = this.browserPatternRepository.FindAll(browserPatterQUery);
            UpdateScreenshotHashes(browserPatterns);
        }

        private void SaveGlobalBlindregions(SaveGlobalBlindRegionsDTO dto)
        {
            if (dto == null)
            {
                return;
            }
            var testCase = this.testCaseRepository.Get(dto.TestCaseId);
            var globalRegionsForBrowser = GetGlobalBlindRegionsForBrowser(testCase.Project, dto.BrowserName);
            globalRegionsForBrowser.ReplaceBlindRegionsSet(dto.BlindRegions);
            var browserPatternQuery = new FindPatternsForBrowserInProject(testCase.Project.Id, dto.BrowserName);
            var browserPatterns = this.browserPatternRepository.FindAll(browserPatternQuery);
            UpdateScreenshotHashes(browserPatterns);
        }

        private static void UpdateScreenshotHashes(List<BrowserPattern> browserPatterns)
        {
            browserPatterns.Select(bp => new
            {
                Screenshot = bp.PatternScreenshot,
                BlindRegions = bp.GetAllBlindRegions()
            }).AsParallel().ForAll(bp =>
            {
                bp.Screenshot.UpdateTestCaseHash(bp.BlindRegions);
            });
        }

        private static BlindRegionForBrowser GetGlobalBlindRegionsForBrowser(IBlindRegionForBrowserOwner blindRegionForBrowserOwner, string browserName)
        {
            var globalRegionsForBrowser = blindRegionForBrowserOwner.GetOwnBlindRegionForBrowser(browserName);
            if (globalRegionsForBrowser == null)
            {
                globalRegionsForBrowser = new BlindRegionForBrowser
                {
                    BrowserName = browserName
                };
                blindRegionForBrowserOwner.AddBlindRegionForBrowser(globalRegionsForBrowser);
            }
            return globalRegionsForBrowser;
        }
       
        public TestCaseCategoriesListViewModel GetTestCaseCategories(long projectId)
        {
            var testCaseCategories = this.testCaseCategoryRepository.FindAll(new FindTestCaseCategoriesFromProject(projectId));
            return new TestCaseCategoriesListViewModel
            {
                Categories = testCaseCategories.ConvertAll(x => new TestCaseCategoryListItem()
                {
                    CategoryId = x.Id,
                    CategoryName = x.Name
                })
            };
        }

        public TestCaseListItem GetTestCase(long id)
        {
            var testCase = this.testCaseRepository.Get(id);
            return MapToTestCaseListItem(testCase);
        }

        public List<TestCaseListItem> GetTestCasesFromCategory(long categoryId)
        {
            var testCases = this.testCaseRepository.FindAll(new FindTestCasesFromCategory(categoryId));
            return testCases.Select(MapToTestCaseListItem).ToList();
        }

        private static TestCaseListItem MapToTestCaseListItem(Tellurium.VisualAssertions.Screenshots.Domain.TestCase x)
        {
            return new TestCaseListItem
            {
                TestCaseId = x.Id,
                TestCaseName = x.PatternScreenshotName,
                Browsers = x.GetActivePatterns()
                    .OrderBy(p=> p.BrowserName)
                    .Select(y => new BrowserPatternShortcut
                    {
                        BrowserName = y.BrowserName,
                        PatternId= y.Id,
                    }).ToList()
            };
        }

        public BrowserPatternDTO GetTestCasePattern(long patternId)
        {
            var browserPattern = browserPatternRepository.Get(patternId);
            var testCase = browserPattern.TestCase;
            var projectBlindRegionForBrowser = testCase.Project.GetOwnBlindRegionForBrowser(browserPattern.BrowserName) ?? new BlindRegionForBrowser();
            var categoryBlindRegionForBrowser = testCase.Category.GetOwnBlindRegionForBrowser(browserPattern.BrowserName) ?? new BlindRegionForBrowser();
            return new BrowserPatternDTO
            {
                GlobalBlindRegions = projectBlindRegionForBrowser.BlindRegions.ToList(),
                CategoryBlindRegions = categoryBlindRegionForBrowser.BlindRegions.ToList(),
                LocalBlindRegions = browserPattern.BlindRegions.ToList(),
                BrowserName = browserPattern.BrowserName,
                PatternId = browserPattern.Id,
                TestCaseId = testCase.Id,
                IsActive = browserPattern.IsActive,
                AllPatterns = MapToAllPatternsDropdown(patternId, testCase, browserPattern)
            };
        }

        private static Dropdown<long> MapToAllPatternsDropdown(long patternId, VisualAssertions.Screenshots.Domain.TestCase testCase, BrowserPattern browserPattern)
        {
            var allPatterns = testCase.Patterns
                .Where(x=>x.BrowserName == browserPattern.BrowserName)
                .OrderByDescending(x=>x.Id).ToList();

            var counter = 0;
            return new Dropdown<long>()
            {
                Selected = patternId,
                Options = allPatterns
                    .Select(x=> new DropdownItem<long>()
                    {
                        Label = (allPatterns.Count - (counter++)).ToString(),
                        Value = x.Id
                    }).ToList()
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
        List<TestCaseListItem> GetTestCasesFromCategory(long categoryId);
        BrowserPatternDTO GetTestCasePattern(long patternId);
        byte[] GetPatternScreenshot(long patternId);
        ProjectListViewModel GetProjectsList();
        TestCaseCategoriesListViewModel GetTestCaseCategories(long projectId);
        TestCaseListItem GetTestCase(long id);
        void SaveBlindRegions(SaveBlindRegionsDTO dto);
    }
}