using System.Collections.Generic;
using System.Linq;
using Tellurium.MvcPages.Configuration;

namespace Tellurium.MvcPages.BrowserCamera.Storage
{
    public static class ScreenshotStorageFactory
    {
        public static IScreenshotStorage CreateForErrorScreenshot(BrowserAdapterConfig adapterConfig)
        {
            var storages = GetAvailableStoragesForError(adapterConfig).ToArray();
            return new CompositeScreenshotStorage(storages);
        }

        private static IEnumerable<IScreenshotStorage> GetAvailableStoragesForError(BrowserAdapterConfig adapterConfig)
        {
            if (TeamCityScreenshotStorage.IsAvailable())
                yield return new TeamCityScreenshotStorage();

            if (string.IsNullOrWhiteSpace(adapterConfig.ScreenshotsPath) == false)
                yield return new FileSystemScreenshotStorage(adapterConfig.ScreenshotsPath);
        }
    }
}