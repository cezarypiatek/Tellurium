using System;
using System.Drawing;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.Utils;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ScrollableLens : IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public ScrollableLens(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public byte[] TakeScreenshot()
        {
            var pageHeight = driver.GetPageHeight();
            var viewPortHeight = driver.GetWindowHeight();
            driver.ScrollToY(0);

            var screenshotFirstPart = GetScreenshotOfVisibleArea();
            if (screenshotFirstPart.Height >= pageHeight)
            {
                return screenshotFirstPart.ToBytes();
            }

            var verticalScrollWidth = driver.GetVerticalScrollWidth();
            var screenshotWidth = screenshotFirstPart.Width-verticalScrollWidth;
            var resultBmp = new Bitmap(screenshotWidth, pageHeight);
            using (var g = Graphics.FromImage(resultBmp))
            {
                g.DrawImage(screenshotFirstPart, 0,0, new Rectangle(0, 0, screenshotWidth, screenshotFirstPart.Height), GraphicsUnit.Pixel);
                var numberOfParts = Math.Ceiling(pageHeight / (double)viewPortHeight);
                for (int partNo = 1; partNo < numberOfParts; partNo++)
                {
                    var yOffset = partNo * viewPortHeight;
                    driver.ScrollToY(yOffset);
                    var screenshotNextPart = GetScreenshotOfVisibleArea();
                    var heightToCover = pageHeight - yOffset;
                    
                    var areaToCopy = heightToCover < viewPortHeight
                        ? new Rectangle(0, screenshotNextPart.Height - heightToCover, screenshotWidth, heightToCover)
                        : new Rectangle(0, 0, screenshotWidth, screenshotNextPart.Height);

                    g.DrawImage(screenshotNextPart, 0, yOffset, areaToCopy, GraphicsUnit.Pixel);
                    
                }
                driver.ScrollToY(0);
                return resultBmp.ToBytes();
            }
        }

        private Bitmap GetScreenshotOfVisibleArea()
        {
            var screenshot = driver.GetScreenshot();
            return screenshot.AsByteArray.ToBitmap();
        }
    }
}