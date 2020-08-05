﻿using System;
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

        /// <summary>
        /// It is possible to implement custom comparison strategy by supplying a class implementing IScreenshotComparisonStrategy interface
        /// </summary>
        public IScreenshotComparisonStrategy ScreenshotComparisonStrategy { get; set; }
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

        public PixelByPixelComparisonParameters(double maxPercentOfUnmatchedPixels)
        {
            MaxPercentOfUnmatchedPixels = maxPercentOfUnmatchedPixels;
            PixelToleranceCount = uint.MaxValue;
        }

        public PixelByPixelComparisonParameters(uint pixelToleranceCount)
        {
            PixelToleranceCount = pixelToleranceCount;
            MaxPercentOfUnmatchedPixels = 100.0;
        }
    }
}