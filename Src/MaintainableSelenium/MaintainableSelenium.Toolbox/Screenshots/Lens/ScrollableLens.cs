using System;
using System.Drawing;
using MaintainableSelenium.Toolbox.SeleniumUtils;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.Screenshots.Lens
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
            driver.ScrollTo(0);

            var screenshotFirstPart = GetScreenshotOfVisibleArea();
            if (screenshotFirstPart.Height >= pageHeight)
            {
                return ImageHelpers.ConvertImageToBytes(screenshotFirstPart);
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
                    driver.ScrollTo(yOffset);
                    var screenshotNextPart = GetScreenshotOfVisibleArea();
                    var heightToCover = pageHeight - yOffset;
                    if (heightToCover < viewPortHeight)
                    {
                        screenshotNextPart = screenshotNextPart.Crop(0, screenshotNextPart.Height - heightToCover, screenshotWidth, heightToCover);
                    }
                    g.DrawImage(screenshotNextPart, 0, yOffset, new Rectangle(0, 0, screenshotWidth, screenshotNextPart.Height), GraphicsUnit.Pixel);
                }
                driver.ScrollTo(0);
                return ImageHelpers.ConvertImageToBytes(resultBmp);
            }
        }

        private Bitmap GetScreenshotOfVisibleArea()
        {
            var screenshot = driver.GetScreenshot();
            return ImageHelpers.ConvertBytesToBitmap(screenshot.AsByteArray);
        }
    }
}