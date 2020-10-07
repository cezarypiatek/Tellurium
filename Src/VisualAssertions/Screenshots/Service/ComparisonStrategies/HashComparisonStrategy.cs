using System;
using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public class HashComparisonStrategy : IScreenshotComparisonStrategy
    {
        public bool Compare(BrowserPattern browserPattern, byte[] screenshot, out string resultMessage)
        {
            if (browserPattern == null)
                throw new ArgumentNullException(nameof(browserPattern));

            if (screenshot == null)
                throw new ArgumentNullException(nameof(screenshot));

            var blindRegions = browserPattern.GetAllBlindRegions();

            var screenshotHash = ImageHelpers.ComputeHash(screenshot, blindRegions);
            var areMatched = screenshotHash == browserPattern.PatternScreenshot.Hash;

            resultMessage = areMatched ? "Hashes match" : "Hashes do not match";

            return areMatched;
        }
    }
}
