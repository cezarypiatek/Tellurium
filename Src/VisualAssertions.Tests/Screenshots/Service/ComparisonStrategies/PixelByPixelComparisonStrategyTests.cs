using Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies;
using System;
using Xunit;
using Tellurium.VisualAssertions.Screenshots.Domain;
using System.Drawing;
using Tellurium.MvcPages.Utils;
using System.Globalization;

namespace VisualAssertions.Tests.Screenshots.Service.ComparisonStrategies
{
    public class PixelByPixelComparisonStrategyTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Compare_ImagesShouldMatchByPixelCount_WhenBitmapsAreExactlyTheSame(uint pixelTolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(pixelTolerance);

            BrowserPattern browserPattern = CreateBrowserPatern(new Bitmap(2, 2));
            var screenshot = new Bitmap(2, 2).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            Assert.True(result);
            Assert.Contains($"+---[PASS] Images match within specified pixel tolerance: 0 <= {pixelTolerance}", resultMessage);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Compare_ImagesShouldMatchByPixelCount_WhenBitmapsAreDifferent(uint pixelTolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(pixelTolerance);

            Bitmap patternBitmap = new Bitmap(3, 3);
            patternBitmap.SetPixel(0, 0, Color.Beige);
            patternBitmap.SetPixel(1, 0, Color.Beige);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var screenshot = new Bitmap(3, 3).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            Assert.True(result);
            Assert.Contains($"+---[PASS] Images match within specified pixel tolerance: 2 <= {pixelTolerance}", resultMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Compare_ImagesShouldNotMatchByPixelCount_WhenBitmapsAreDifferent(uint pixelTolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(pixelTolerance);

            Bitmap patternBitmap = new Bitmap(3, 3);
            patternBitmap.SetPixel(0, 0, Color.Beige);
            patternBitmap.SetPixel(1, 0, Color.Beige);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var screenshot = new Bitmap(3, 3).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            Assert.False(result);
            Assert.Contains($"+---[FAIL] Images do not match within specified pixel tolerance: 2 <= {pixelTolerance}", resultMessage);
        }


        [Theory]
        [InlineData(0.1)]
        [InlineData(1.0)]
        [InlineData(100.0)]
        public void Compare_ImagesShouldMatchWithinToleranceSpecifiedInPercent_WhenBitmapsAreExactlyTheSame(decimal tolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(tolerance);

            BrowserPattern browserPattern = CreateBrowserPatern(new Bitmap(2, 2));
            var screenshot = new Bitmap(2, 2).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            var toleranceStr = tolerance.ToString(CultureInfo.InvariantCulture);
            Assert.True(result);
            Assert.Contains($"+---[PASS] Images match within specified percent of tolerance: 0.0% <= {toleranceStr}% (0px/4px)", resultMessage);
        }

        [Theory]
        [InlineData(11.11)]
        [InlineData(11.12)]
        [InlineData(11.2)]
        [InlineData(11.234)]
        [InlineData(100.0)]
        public void Compare_ImagesShouldMatchWithinToleranceSpecifiedInPercent_WhenBitmapsAreDifferent(decimal tolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(tolerance);

            Bitmap patternBitmap = new Bitmap(3, 3);
            patternBitmap.SetPixel(0, 0, Color.Beige);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var screenshot = new Bitmap(3, 3).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            var toleranceStr = tolerance.ToString(CultureInfo.InvariantCulture);
            Assert.True(result);
            Assert.Contains($"+---[PASS] Images match within specified percent of tolerance: 11.11% <= {toleranceStr}% (1px/9px)", resultMessage);
        }

        [Theory]
        [InlineData(11.1)]
        [InlineData(0.0)]
        public void Compare_ImagesShouldNotMatchWithinToleranceSpecifiedInPercent_WhenBitmapsAreDifferent(decimal tolerance)
        {
            var sut = new PixelByPixelComparisonStrategy(tolerance);

            Bitmap patternBitmap = new Bitmap(3, 3);
            patternBitmap.SetPixel(0, 0, Color.Beige);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var screenshot = new Bitmap(3, 3).ToBytes();

            var result = sut.Compare(browserPattern, screenshot, out var resultMessage);

            var toleranceStr = tolerance.ToString(CultureInfo.InvariantCulture);
            Assert.False(result);
            Assert.Contains($"+---[FAIL] Images do not match within specified percent of tolerance: 11.11% <= {toleranceStr}% (1px/9px)", resultMessage);
        }


        [Fact]
        public void Constructor_CantCreatePixelByPixelComparisonStrategy_WhenToleranceIsLessThanZeroPercent()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PixelByPixelComparisonStrategy(-0.01m));
        }

        [Fact]
        public void Constructor_CantCreatePixelByPixelComparisonStrategy_WhenToleranceIsHigherThan100Percent()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PixelByPixelComparisonStrategy(100.01m));
        }

        private static BrowserPattern CreateBrowserPatern(Bitmap bitmap)
        {
            return new BrowserPattern()
            {
                BrowserName = "DummyBrowser",
                TestCase = new TestCase()
                {
                    Category = new TestCaseCategory()
                    {
                        Name = "DummyCategoryName",
                        Project = new Project() { Name = "DummyProjectName" }
                    },
                    Project = new Project() { Name = "DummyProjectName" }
                },
                PatternScreenshot = new ScreenshotData() { Image = bitmap.ToBytes() }
            };
        }

        [Fact]
        public void CannotCallCompareWithNullBrowserPattern()
        {
            var sut = new PixelByPixelComparisonStrategy(0.1m);
            Assert.Throws<ArgumentNullException>(() => sut.Compare(default(BrowserPattern), new byte[] { 20, 210, 200, 188 }, out _));
        }

        [Fact]
        public void CannotCallCompareWithNullScreenshot()
        {
            var sut = new PixelByPixelComparisonStrategy(0.1m);
            Assert.Throws<ArgumentNullException>(() => sut.Compare(new BrowserPattern(), default(byte[]), out _));
        }
    }
}