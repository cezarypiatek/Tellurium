using Tellurium.MvcPages.BrowserCamera.Lens;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera
{
    internal class ErrorBrowserCamera:IBrowserCamera
    {
        private readonly IBrowserCameraLens lens;

        public ErrorBrowserCamera(IBrowserCameraLens lens)
        {
            this.lens =  lens;
        }

        public byte[] TakeScreenshot()
        {
            return ExceptionHelper.SwallowException(() =>  lens.TakeScreenshot() , null);
        }
    }
}