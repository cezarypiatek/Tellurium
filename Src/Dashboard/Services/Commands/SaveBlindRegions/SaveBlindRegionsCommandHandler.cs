using System.Collections.Generic;
using System.Linq;
using Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegions;
using Tellurium.VisualAssertion.Dashboard.Services.WorkSeed;
using Tellurium.VisualAssertions.Infrastructure;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Queries;

namespace Tellurium.VisualAssertion.Dashboard.Services.Commands.SaveBlindRegion
{
    public class SaveBlindRegionsCommandHandler:ICommandHandler<SaveBlindRegionsCommand>
    {
        private readonly IRepository<TestCase> testCaseRepository;
        private readonly IRepository<BrowserPattern> browserPatternRepository;

        public SaveBlindRegionsCommandHandler(IRepository<TestCase> testCaseRepository, IRepository<BrowserPattern> browserPatternRepository)
        {
            this.testCaseRepository = testCaseRepository;
            this.browserPatternRepository = browserPatternRepository;
        }

        public void Excute(SaveBlindRegionsCommand command)
        {
            SaveLocalBlindregions(command.Local);
            SaveCategoryBlindregions(command.Category);
            SaveGlobalBlindregions(command.Global);
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
    }
}