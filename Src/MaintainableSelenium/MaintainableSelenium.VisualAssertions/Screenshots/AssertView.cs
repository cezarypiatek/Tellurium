using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.VisualAssertions.Infrastructure.Persistence;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;
using MaintainableSelenium.VisualAssertions.TestRunersAdapters;

namespace MaintainableSelenium.VisualAssertions.Screenshots
{
    public static class AssertView
    {
        private static VisualAssertionsService visualAssertionsService;

        public static void Init(VisualAssertionsConfig config)
        {
            var testRunnerAdapter = TestRunnerAdapterFactory.CreateForCurrentEnvironment();
            var projectRepository = new Repository<Project>(PersistanceEngine.GetSessionContext());
            visualAssertionsService = new VisualAssertionsService(projectRepository,testRunnerAdapter)
            {
                ProjectName = config.ProjectName,
                ScreenshotCategory = config.ScreenshotCategory,
                BrowserName = config.BrowserName
            };
        }

        public static void EqualsToPattern(IBrowserCamera browserCamera, string viewName)
        {
            visualAssertionsService.CheckViewWithPattern(browserCamera, viewName);
        }
    }
}