using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service.ComparisonStrategies
{
    public interface IScreenshotComparisonStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="browserPattern"></param>
        /// <param name="screenshot"></param>
        /// <returns>true if images match compare using the specified comparison strategy</returns>
        bool Compare(BrowserPattern browserPattern, byte[] screenshot);
    }
}
