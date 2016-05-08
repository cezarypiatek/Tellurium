using MaintainableSelenium.MvcPages.BrowserCamera;
using MaintainableSelenium.VisualAssertions.Infrastructure.Persistence;
using MaintainableSelenium.VisualAssertions.Screenshots.Domain;

namespace MaintainableSelenium.VisualAssertions.Screenshots
{
    public static class VisualAssertionScreenshotStorageFactory
    {
        public static IScreenshotStorage Create()
        {
            return new ScreenshotService(new Repository<Project>(PersistanceEngine.GetSessionContext()));
        }
    }
}