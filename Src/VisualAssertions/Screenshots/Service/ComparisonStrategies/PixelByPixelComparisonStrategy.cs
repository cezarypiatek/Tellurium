using System.Collections.Generic;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class PixelByPixelComparisonStrategy : IScreenshotComparisonStrategy
    {
        public int PixelTolerance { get; }
        public byte PixelColorTolerance { get; }

        public PixelByPixelComparisonStrategy(int pixelTolerance = 0, byte pixelColorTolerance = 0)
        {
            PixelTolerance = pixelTolerance;
            PixelColorTolerance = pixelColorTolerance;
        }

        public bool Compare(BrowserPattern browserPattern, byte[] screenshot)
        {
            var pattern = browserPattern.PatternScreenshot.Image;
            var blindRegions = browserPattern.GetAllBlindRegions();
            return ComparePixelByPixel(pattern, screenshot, blindRegions);
        }

        /// <summary>
        /// Compares images pixel by pixel with a specified tolerance defined by PixelTolerance and PixelColorTolerance
        /// </summary>
        /// <param name="screenshot"></param>
        /// <param name="blindRegions"></param>
        /// <returns>true if images match</returns>
        public bool ComparePixelByPixel(byte[] pattern, byte[] screenshot,
            IReadOnlyList<BlindRegion> blindRegions)
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

            return numberOfUnmatchedPixels < PixelTolerance;
        }

        ///// <summary>
        ///// Compares images pixel by pixel with a specified tolerance defined by PixelTolerance and PixelColorTolerance
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

        //            if (PixelColorTolerance == 0 || Math.Abs(imageA[alpha] - imageB[alpha]) > PixelColorTolerance ||
        //                Math.Abs(imageA[red] - imageB[red]) > PixelColorTolerance ||
        //                Math.Abs(imageA[green] - imageB[green]) > PixelColorTolerance ||
        //                Math.Abs(imageA[blue] - imageB[blue]) > PixelColorTolerance)
        //            {
        //                numberOfUnmatchedPixels++;
        //            }
        //        }
        //    }

        //    return numberOfUnmatchedPixels < PixelTolerance;
        //}
    }
}
