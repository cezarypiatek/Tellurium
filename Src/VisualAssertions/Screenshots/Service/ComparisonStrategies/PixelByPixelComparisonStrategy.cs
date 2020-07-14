using System.Collections.Generic;
using System.Globalization;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class PixelByPixelComparisonStrategy : IScreenshotComparisonStrategy
    {
        public PixelByPixelComparisonOptions ComparisonOptions { get; }

        public PixelByPixelComparisonStrategy(PixelByPixelComparisonOptions comparisonOptions)
        {
            ComparisonOptions = comparisonOptions;
        }

        /// <summary>
        /// Compares images pixel by pixel with a specified tolerance defined by PixelToleranceCount and PixelColorToleranceCount
        /// </summary>
        public bool Compare(BrowserPattern browserPattern, byte[] screenshot, out string resultMessage)
        {
            var pattern = browserPattern.PatternScreenshot.Image;
            var blindRegions = browserPattern.GetAllBlindRegions();
            var numberOfUnmatchedPixels = CountUnmatchedPixels(pattern, screenshot, blindRegions);

            var percentOfUnmatchedPixels = (double)numberOfUnmatchedPixels / (pattern.Length / 4.0) * 100;
            var areMatched = numberOfUnmatchedPixels < ComparisonOptions.PixelToleranceCount && percentOfUnmatchedPixels < ComparisonOptions.MaxPercentOfUnmatchedPixels;


            return areMatched;
        }

        private static int CountUnmatchedPixels(byte[] pattern, byte[] screenshot, IReadOnlyList<BlindRegion> blindRegions)
        {
            var numberOfUnmatchedPixels = 0;
            var imageA = ImageHelpers.ApplyBlindRegions(pattern, blindRegions);
            var imageB = ImageHelpers.ApplyBlindRegions(screenshot, blindRegions);

            for (int k = 0; k + 4 < imageB.Length; k += 4)
            {
                if (imageA[k] != imageB[k] ||
                    imageA[k + 1] != imageB[k + 1] ||
                    imageA[k + 2] != imageB[k + 2] ||
                    imageA[k + 3] != imageB[k + 3])
                {
                    numberOfUnmatchedPixels++;
                }
            }

            return numberOfUnmatchedPixels;
        }

        ///// <summary>
        ///// Compares images pixel by pixel with a specified tolerance defined by PixelToleranceCount and PixelColorToleranceCount
        ///// </summary>
        ///// <param name="screenshot"></param>
        ///// <param name="blindRegions"></param>
        ///// <returns>true if images match</returns>
        //public bool ComparePixelByPixel(byte[] pattern, byte[] screenshot,
        //    IReadOnlyList<BlindRegion> blindRegions)
        //{
        //    var numberOfUnmatchedPixels = 0;
        //    var imageA = ImageHelpers.ApplyBlindRegions(pattern, blindRegions);
        //    var imageB = ImageHelpers.ApplyBlindRegions(screenshot, blindRegions);

        //    for (int k = 0; k + 4 < imageB.Length; k += 4)
        //    {
        //        var alpha = k;
        //        var red = k + 1;
        //        var green = k + 2;
        //        var blue = k + 3;

        //        if (imageA[alpha] != imageB[alpha] ||
        //            imageA[red] != imageB[red] ||
        //            imageA[green] != imageB[green] ||
        //            imageA[blue] != imageB[blue])
        //        {

        //            if (PixelColorToleranceCount == 0 || Math.Abs(imageA[alpha] - imageB[alpha]) > PixelColorToleranceCount ||
        //                Math.Abs(imageA[red] - imageB[red]) > PixelColorToleranceCount ||
        //                Math.Abs(imageA[green] - imageB[green]) > PixelColorToleranceCount ||
        //                Math.Abs(imageA[blue] - imageB[blue]) > PixelColorToleranceCount)
        //            {
        //                numberOfUnmatchedPixels++;
        //            }
        //        }
        //    }

        //    return numberOfUnmatchedPixels < PixelToleranceCount;
        //}
    }
}
