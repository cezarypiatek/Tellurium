using System;
using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure.Persistence;
using Tellurium.VisualAssertions.Screenshots.Domain;
using Tellurium.VisualAssertions.Screenshots.Service;
using Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies;
using Tellurium.VisualAssertions.TestRunersAdapters;

namespace Tellurium.VisualAssertions.Screenshots
{
    public static class AssertView
    {
        private static VisualAssertionsService _visualAssertionsService;

        public static void Init(VisualAssertionsConfig config)
        {
            var testOutputWriter = config.TestOutputWriter ?? Console.WriteLine;
            var testRunnerAdapter = TestRunnerAdapterFactory.CreateForCurrentEnvironment(testOutputWriter);
            var sessionContext = PersistanceEngine.GetSessionContext();
            var projectRepository = new Repository<Project>(sessionContext);

            IScreenshotComparisonStrategy comparisonStrategy;
            switch (config.ComparisonStrategy)
            {
                case ComparisonStrategy.PixelByPixel:
                    comparisonStrategy = new PixelByPixelComparisonStrategy(config.PixelByPixelComparisonOptions);
                    break;
                case ComparisonStrategy.Hash:
                    comparisonStrategy = new HashComparisonStrategy();
                    break;
                default:
                    comparisonStrategy = new HashComparisonStrategy();
                    break;
            }

            _visualAssertionsService?.Dispose();
            _visualAssertionsService = new VisualAssertionsService(projectRepository, testRunnerAdapter, config.ProcessScreenshotsAsynchronously, comparisonStrategy)
            {
                ProjectName = config.ProjectName,
                ScreenshotCategory = config.ScreenshotCategory,
                BrowserName = config.BrowserName
            };
        }

        public static void EqualsToPattern(IBrowserCamera browserCamera, string viewName)
        {
            _visualAssertionsService.CheckViewWithPattern(browserCamera, viewName);
        }
    }
}