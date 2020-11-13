using Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies;
using System;
using Xunit;
using Tellurium.VisualAssertions.Screenshots.Domain;
using System.Drawing;
using Tellurium.MvcPages.Utils;
using System.Globalization;
using Tellurium.VisualAssertions.Screenshots;

namespace VisualAssertions.Tests.Screenshots.Service.ComparisonStrategies
{
    public class HashComparisonStrategyTests
    {
        private HashComparisonStrategy _testClass;

        public HashComparisonStrategyTests()
        {
            _testClass = new HashComparisonStrategy();
        }

        [Fact]
        public void CanConstruct()
        {
            var instance = new HashComparisonStrategy();
            Assert.NotNull(instance);
        }

        [Fact]
        public void Compare_ReturnsFalse_ForDifferentBitmaps()
        {
            Bitmap patternBitmap = new Bitmap(3, 3);
            patternBitmap.SetPixel(0, 0, Color.Beige);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var blindRegions = browserPattern.GetAllBlindRegions();
            var screenshot = new Bitmap(3, 3).ToBytes();
            browserPattern.PatternScreenshot.UpdateHash(blindRegions);


            var result = _testClass.Compare(browserPattern, screenshot, out var resultMessage);
            Assert.False(result);
        }

        [Fact]
        public void Compare_ReturnsTrue_ForMatchingBitmaps()
        {
            Bitmap patternBitmap = new Bitmap(3, 3);
            BrowserPattern browserPattern = CreateBrowserPatern(patternBitmap);
            var blindRegions = browserPattern.GetAllBlindRegions();
            var screenshot = new Bitmap(3, 3).ToBytes();
            browserPattern.PatternScreenshot.UpdateHash(blindRegions);


            var result = _testClass.Compare(browserPattern, screenshot, out var resultMessage);
            Assert.True(result);
        }

        [Fact]
        public void CannotCallCompareWithNullBrowserPattern()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Compare(default(BrowserPattern), new byte[] { 115, 250, 158, 110 }, out _));
        }

        [Fact]
        public void CannotCallCompareWithNullScreenshot()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Compare(new BrowserPattern(), default(byte[]), out _));
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
    }
}