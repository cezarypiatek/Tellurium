using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public interface IScreenshotComparisonStrategy
    {
        /// <summary>
        /// It is possible to implement custom comparison strategy by supplying a class implementing IScreenshotComparisonStrategy interface
        /// </summary>
        /// <param name="browserPattern"></param>
        /// <param name="screenshot"></param>
        /// <param name="resultMessage"></param>
        /// <returns>true if images match compare using the specified comparison strategy</returns>
        bool Compare(BrowserPattern browserPattern, byte[] screenshot, out string resultMessage);
    }
}
