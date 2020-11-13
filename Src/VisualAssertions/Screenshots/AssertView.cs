using System;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Service;
using Tellurium.VisualAssertions.TestRunnersAdapters;

namespace Tellurium.VisualAssertions.Screenshots
{
    public static class AssertView
    {
        private static VisualAssertionsService _visualAssertionsService;

        public static void Init(VisualAssertionsConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.ScreenshotComparisonStrategy == null)
                throw new InvalidOperationException($"{nameof(config.ScreenshotComparisonStrategy)} must be specified");

            var testOutputWriter = config.TestOutputWriter ?? Console.WriteLine;
            var testRunnerAdapter = TestRunnerAdapterFactory.CreateForCurrentEnvironment(testOutputWriter);
            var sessionContext = PersistanceEngine.GetSessionContext();
            var projectRepository = new Repository<Project>(sessionContext);
            var comparisonStrategy = config.ScreenshotComparisonStrategy;

            _visualAssertionsService?.Dispose();
            _visualAssertionsService = new VisualAssertionsService(
                projectRepository,
                testRunnerAdapter,
                config.ProcessScreenshotsAsynchronously,
                config.ProjectName,
                config.ScreenshotCategory,
                config.BrowserName,
                comparisonStrategy)
            {
            };
        }

        public static void EqualsToPattern(IBrowserCamera browserCamera, string viewName)
        {
            _visualAssertionsService.CheckViewWithPattern(browserCamera, viewName);
        }
    }
}