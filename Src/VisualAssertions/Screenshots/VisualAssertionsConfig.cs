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

        public PixelByPixelComparisonParameters PixelByPixelComparisonParameters { get; set; }
    }

    public class PixelByPixelComparisonParameters
    {
        /// <summary>
        /// Percentage of pixels that can be different when matching two images.
        /// If percentage of pixels that are different is greater than this value then the images are considered unmatched
        /// </summary>
        /// <remarks>
        /// Accepted values: <0.0, 100.0>
        /// Only applicable if ComparisonStrategy is PixelByPixel
        /// </remarks>
        public double MaxPercentOfUnmatchedPixels { get; }

        /// <summary>
        /// Number of pixels that can be different when matching two images.
        /// If number of pixels that are different is greater than this value then the images are considered unmatched
        /// </summary>
        /// <remarks>Only applicable if ComparisonStrategy is PixelByPixel</remarks>
        public uint PixelToleranceCount { get; }

        /// <summary>
        /// Maximimum difference by which sum of values from all channels (alpha, red, green, blue) can differ
        /// If the sum is greater than the specified value then the images are considered unmatched 
        /// </summary>
        /// <remarks>Only applicable if ComparisonStrategy is PixelByPixel</remarks>
        public uint PixelColorToleranceCount { get; }

        public PixelByPixelComparisonParameters(double maxPercentOfUnmatchedPixels, uint pixelColorToleranceCount = 0)
        {
            PixelColorToleranceCount = uint.MaxValue;
            MaxPercentOfUnmatchedPixels = maxPercentOfUnmatchedPixels;
            PixelColorToleranceCount = pixelColorToleranceCount;
        }

        public PixelByPixelComparisonParameters(uint pixelToleranceCount, uint pixelColorToleranceCount = 0)
        {
            PixelToleranceCount = pixelToleranceCount;
            MaxPercentOfUnmatchedPixels = 100.0;
            PixelColorToleranceCount = pixelColorToleranceCount;
        }
    }

    public enum ComparisonStrategy
    {
        Hash = 0,
        PixelByPixel = 1
    }
}