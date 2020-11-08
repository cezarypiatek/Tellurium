using System;
using System.Drawing;
using System.Drawing.Imaging;
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
            if (browserPattern == null)
                throw new ArgumentNullException(nameof(browserPattern));

            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));

            var pattern = browserPattern.PatternScreenshot.Image;
            var blindRegions = browserPattern.GetAllBlindRegions();
            var patternBitmap = ImageHelpers.ApplyBlindRegions(pattern, blindRegions);
            var screenshotBitmap = ImageHelpers.ApplyBlindRegions(screenshot, blindRegions);

            if (patternBitmap == null)
            {
                resultMessage = "\r\n+---patternBitmap is null";
                return false;
            }

            if (screenshotBitmap == null)
            {
                resultMessage = "\r\n+---screenshotBitmap is null";
                return false;
            }

            if (!AreBitmapsOfEqualSize(patternBitmap, screenshotBitmap))
            {
                resultMessage = "\r\n+---Different sizes of bitmaps" +
                                $"\r\n+---Pattern size: {patternBitmap.Size}" +
                                $"\r\n+---Screenshot size: {screenshotBitmap.Size}";

                return false;
            }

            var numberOfUnmatchedPixels = CountUnmatchedPixels(patternBitmap, screenshotBitmap);

            var percentOfUnmatchedPixels = (numberOfUnmatchedPixels * 100.0) / (patternBitmap.Width * patternBitmap.Height) ;
            var areMatchedByPixelCount = numberOfUnmatchedPixels <= ComparisonParameters.PixelToleranceCount;
            var areMatchedByAllowedMaximumDifferenceInPercent = percentOfUnmatchedPixels <= ComparisonParameters.MaxPercentOfUnmatchedPixels;

            resultMessage = "\r\n+---" + (areMatchedByPixelCount ? "[PASS] Images match" : "[FAIL] Images do not match") +
                            $" within specified pixel tolerance: {numberOfUnmatchedPixels}" +
                            $" <= {ComparisonParameters.PixelToleranceCount}";

            resultMessage += $"\r\n+---" + (areMatchedByAllowedMaximumDifferenceInPercent ? "[PASS] Images match" : "[FAIL] Images do not match") +
                            " within specified percent of tolerance: " +
                            $"{percentOfUnmatchedPixels.ToString("0.######", CultureInfo.InvariantCulture)}%" +
                            " <= " +
                            $"{ComparisonParameters.MaxPercentOfUnmatchedPixels.ToString("0.######", CultureInfo.InvariantCulture)}%";

            return areMatchedByPixelCount && areMatchedByAllowedMaximumDifferenceInPercent;
        }

        private static bool AreBitmapsOfEqualSize(Bitmap patternBitmap, Bitmap screenshotBitmap)
        {
            return patternBitmap.Height == screenshotBitmap.Height && patternBitmap.Width == screenshotBitmap.Width;
        }

        /// <summary>
        /// This method assumes that the bitmaps are of equal size in terms of pixels
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="screenshot"></param>
        /// <returns></returns>
        public int CountUnmatchedPixels(Bitmap pattern, Bitmap screenshot)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));

            if (!AreBitmapsOfEqualSize(pattern, screenshot))
                throw new ArgumentException("Bitmaps have different sizes");

            var numberOfUnmatchedPixels = 0;

            BitmapData patternBitmapData = pattern.LockBits(new Rectangle(0, 0, pattern.Width, pattern.Height), ImageLockMode.ReadOnly, pattern.PixelFormat);
            byte bitsPerPixel = GetBitsPerPixel(patternBitmapData.PixelFormat);
            int patternSize = patternBitmapData.Stride * patternBitmapData.Height;
            byte[] patternData = new byte[patternSize];
            System.Runtime.InteropServices.Marshal.Copy(patternBitmapData.Scan0, patternData, 0, patternSize);


            BitmapData screenshotBitmapData = screenshot.LockBits(new Rectangle(0, 0, screenshot.Width, screenshot.Height), ImageLockMode.ReadOnly, screenshot.PixelFormat);
            int screenshotSize = screenshotBitmapData.Stride * screenshotBitmapData.Height;
            byte[] screenshotData = new byte[screenshotSize];
            System.Runtime.InteropServices.Marshal.Copy(screenshotBitmapData.Scan0, screenshotData, 0, screenshotSize);

            for (int i = 0; i < patternSize; i += bitsPerPixel / 8)
            {
                if (patternData[i] != screenshotData[i])
                    numberOfUnmatchedPixels++;
            }

            pattern.UnlockBits(patternBitmapData);
            screenshot.UnlockBits(screenshotBitmapData);

            return numberOfUnmatchedPixels;
        }

        private byte GetBitsPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 24;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 32;
                    break;
                default:
                    throw new ArgumentException("Only 24 and 32 bit images are supported");
            }
        }
    }
}
