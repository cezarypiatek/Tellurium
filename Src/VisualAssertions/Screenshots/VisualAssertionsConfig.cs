using System;

namespace Tellurium.VisualAssertions.Screenshots
{
    public class VisualAssertionsConfig
    {
        public string BrowserName { get; set; }
        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public Action<string> TestOutputWriter { get; set; }
        public bool ProcessScreenshotsAsynchronously { get; set; }

        public ComparisonStrategy ComparisonStrategy { get; set; }

        /// <summary>
        /// Only applicable if ComparisonStrategy is PixelByPixel
        /// </summary>
        public int PixelTolerance { get; set; }

        /// <summary>
        /// Only applicable if ComparisonStrategy is PixelByPixel
        /// </summary>
        public int PixelColorTolerance { get; set; }
    }

    public enum ComparisonStrategy
    {
        Hash = 0,
        PixelByPixel = 1
    }
}