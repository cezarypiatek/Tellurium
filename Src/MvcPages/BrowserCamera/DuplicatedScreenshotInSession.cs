using System;

namespace Tellurium.MvcPages.BrowserCamera
{
    public class DuplicatedScreenshotInSession: ApplicationException
    {
        private readonly ScreenshotIdentity screenshotIdentity;

        public override string Message => $"Cannot take twice the same screenshot. Duplicated screenshot: {screenshotIdentity.FullName}";

        public DuplicatedScreenshotInSession(ScreenshotIdentity screenshotIdentity)
        {
            this.screenshotIdentity = screenshotIdentity;
        }
    }
}