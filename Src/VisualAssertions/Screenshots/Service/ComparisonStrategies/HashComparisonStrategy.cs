using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class HashComparisonStrategy : IScreenshotComparisonStrategy
    {
        public bool Compare(BrowserPattern browserPattern, byte[] screenshot)
        {
            var blindRegions = browserPattern.GetAllBlindRegions();

            var screenshotHash = ImageHelpers.ComputeHash(screenshot, blindRegions);
            return screenshotHash == browserPattern.PatternScreenshot.Hash;
        }
    }
}
