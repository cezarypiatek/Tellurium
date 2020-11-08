using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class PixelByPixelComparisonStrategy : IScreenshotComparisonStrategy
    {
        /// <summary>
        /// Percentage of pixels that can be different when matching two images.
        /// If percentage of pixels that are different is greater than this value then the images are considered unmatched
        /// </summary>
        /// <remarks>
        /// Accepted values: <0.0, 100.0>
        /// </remarks>
        public decimal? MaxPercentOfUnmatchedPixels { get; }

        /// <summary>
        /// Number of pixels that can be different when matching two images.
        /// If number of pixels that are different is greater than this value then the images are considered unmatched
        /// </summary>
        public uint? PixelToleranceCount { get; }

        public PixelByPixelComparisonStrategy(decimal maxPercentOfUnmatchedPixels)
        {
            if (maxPercentOfUnmatchedPixels < 0)
                throw new ArgumentOutOfRangeException($"{nameof(maxPercentOfUnmatchedPixels)} cannot be less than zero");

            if (maxPercentOfUnmatchedPixels > 100)
                throw new ArgumentOutOfRangeException($"{nameof(maxPercentOfUnmatchedPixels)} cannot be higher than 100");

            MaxPercentOfUnmatchedPixels = maxPercentOfUnmatchedPixels;
        }

        public PixelByPixelComparisonStrategy(uint pixelToleranceCount)
        {
            PixelToleranceCount = pixelToleranceCount;
        }

        public PixelByPixelComparisonStrategy(decimal maxPercentOfUnmatchedPixels, uint pixelToleranceCount)
        {
            MaxPercentOfUnmatchedPixels = maxPercentOfUnmatchedPixels;
            PixelToleranceCount = pixelToleranceCount;
        }

        /// <summary>
        /// Compares images pixel by pixel with a specified tolerance defined by PixelToleranceCount and/or PixelColorToleranceCount
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

            var umatchedPixelsCount = CountUnmatchedPixels(patternBitmap, screenshotBitmap);

            resultMessage = "";

            bool areMatched = false;
            if (PixelToleranceCount != null)
            {
                var areMatchedByPixelCount = umatchedPixelsCount <= PixelToleranceCount;
                resultMessage += "\r\n+---" + (areMatchedByPixelCount ? "[PASS] Images match" : "[FAIL] Images do not match") +
                                $" within specified pixel tolerance: {umatchedPixelsCount} <= {PixelToleranceCount}";
                areMatched = areMatchedByPixelCount;
            }

            if (MaxPercentOfUnmatchedPixels != null)
            {
                int pixelsTotalCount = patternBitmap.Width * patternBitmap.Height;
                var unmatchedPixelsPercent = (umatchedPixelsCount * 100.0m) / pixelsTotalCount;
                unmatchedPixelsPercent = Math.Round(unmatchedPixelsPercent, 2);
                var areMatchedByAllowedMaximumDifferenceInPercent = unmatchedPixelsPercent <= MaxPercentOfUnmatchedPixels;

                resultMessage += $"\r\n+---" + (areMatchedByAllowedMaximumDifferenceInPercent ? "[PASS] Images match" : "[FAIL] Images do not match") +
                                " within specified percent of tolerance: " +
                                $"{unmatchedPixelsPercent.ToString(CultureInfo.InvariantCulture)}%" +
                                " <= " +
                                $"{((decimal) MaxPercentOfUnmatchedPixels).ToString(CultureInfo.InvariantCulture)}% ({umatchedPixelsCount}px/{pixelsTotalCount}px)";
                areMatched = areMatched || areMatchedByAllowedMaximumDifferenceInPercent;
            }

            return areMatched;
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
        private int CountUnmatchedPixels(Bitmap pattern, Bitmap screenshot)
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
