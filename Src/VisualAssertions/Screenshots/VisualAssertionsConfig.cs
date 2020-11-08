using System;
using Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class VisualAssertionsConfig
    {
        public string BrowserName { get; set; }
        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public Action<string> TestOutputWriter { get; set; }
        public bool ProcessScreenshotsAsynchronously { get; set; }
        public IScreenshotComparisonStrategy ScreenshotComparisonStrategy { get; set; }
    }
}