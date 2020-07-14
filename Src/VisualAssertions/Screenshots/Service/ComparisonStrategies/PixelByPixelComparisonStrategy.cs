using System;
using System.Collections.Generic;
using System.Globalization;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class PixelByPixelComparisonStrategy : IScreenshotComparisonStrategy
    {
        public PixelByPixelComparisonParameters ComparisonParameters { get; }

        public PixelByPixelComparisonStrategy(PixelByPixelComparisonParameters comparisonParameters)
        {
            ComparisonParameters = comparisonParameters;
        }

        /// <summary>
        /// Compares images pixel by pixel with a specified tolerance defined by PixelToleranceCount and PixelColorToleranceCount
        /// </summary>
        public bool Compare(BrowserPattern browserPattern, byte[] screenshot, out string resultMessage)
        {
            var pattern = browserPattern.PatternScreenshot.Image;
            var blindRegions = browserPattern.GetAllBlindRegions();
            var numberOfUnmatchedPixels = CountUnmatchedPixels(pattern, screenshot, blindRegions);

            const int numberOfChannels = 4;
            var percentOfUnmatchedPixels = (double) numberOfUnmatchedPixels / pattern.Length / numberOfChannels;
            var areMatched = numberOfUnmatchedPixels < ComparisonParameters.PixelToleranceCount && percentOfUnmatchedPixels < ComparisonParameters.MaxPercentOfUnmatchedPixels;


            return areMatched;
        }

        private int CountUnmatchedPixels(byte[] pattern, byte[] screenshot, IReadOnlyList<BlindRegion> blindRegions)
        {
            var numberOfUnmatchedPixels = 0;
            var imageA = ImageHelpers.ApplyBlindRegions(pattern, blindRegions);
            var imageB = ImageHelpers.ApplyBlindRegions(screenshot, blindRegions);


            for (int k = 0; k + 4 < imageB.Length; k += 4)
            {
                var alpha = k;
                var red = k + 1;
                var green = k + 2;
                var blue = k + 3;

                var colorDifference = Math.Abs(imageA[alpha] - imageB[alpha]) +
                                      Math.Abs(imageA[red] - imageB[red]) +
                                      Math.Abs(imageA[green] - imageB[green]) +
                                      Math.Abs(imageA[blue] - imageB[blue]);

                if (colorDifference > ComparisonParameters.PixelColorToleranceCount || 
                    imageA[alpha] != imageB[alpha] ||
                    imageA[red] != imageB[red] ||
                    imageA[green] != imageB[green] ||
                    imageA[blue] != imageB[blue])
                {
                    numberOfUnmatchedPixels++;
                }
            }

            return numberOfUnmatchedPixels;
        }
    }
}
