using System;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.TestRunersAdapters;

namespace Tellurium.VisualAssertions.Screenshots
{
    public static class AssertView
    {
        private static VisualAssertionsService visualAssertionsService;

        public static void Init(VisualAssertionsConfig config)
        {
            var testOutputWriter = config.TestOutputWriter ?? Console.WriteLine;
            var testRunnerAdapter = TestRunnerAdapterFactory.CreateForCurrentEnvironment(testOutputWriter);
            var sessionContext = PersistanceEngine.GetSessionContext();
            var projectRepository = new Repository<Project>(sessionContext);
            visualAssertionsService?.Dispose();
            visualAssertionsService = new VisualAssertionsService(projectRepository,testRunnerAdapter, config.ProcessScreenshotsAsynchronously)
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