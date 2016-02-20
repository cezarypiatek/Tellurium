using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Responsible for taking screenshots of the page
    /// </summary>
    public class BrowserCamera : IBrowserCamera
    {
        private readonly RemoteWebDriver driver;
        private readonly string imageNamePrefix;
        private readonly string outputPath;
        private readonly List<BlindRegion> blindRegions;
        private int counter;
        public BrowserCamera(RemoteWebDriver driver, string imageNamePrefix, string outputPath, List<BlindRegion> blindRegions = null)
        {
            this.driver = driver;
            this.imageNamePrefix = imageNamePrefix;
            this.outputPath = outputPath;
            this.blindRegions = blindRegions ?? new List<BlindRegion>();
        }

        public void TakeScreenshot(string name)
        {
            driver.Blur();
            var screenshot = driver.GetScreenshot();
            counter++;
            var patternPath =  string.Format("{0}\\{1}_{2:D5}_{3}_PATTERN.png", outputPath, imageNamePrefix, counter, name);
            var errorPath = string.Format("{0}\\{1}_{2:D5}_{3}_ERROR.png", outputPath, imageNamePrefix, counter, name);
            if (File.Exists(patternPath))
            {
                SaveScreenshot(screenshot, errorPath);
                if (ComputeHash(errorPath) == ComputeHash(patternPath))
                {
                    File.Delete(errorPath);
                }
            }
            else
            {
                SaveScreenshot(screenshot, patternPath);
            }
        }

        private void SaveScreenshot(Screenshot screenshot, string path)
        {
            using (MemoryStream memoryStream = new MemoryStream(screenshot.AsByteArray))
            {
                var image = Image.FromStream(memoryStream);
                var graphic = Graphics.FromImage(image);
                foreach (var blindRegion in blindRegions)
                {
                    graphic.FillRectangle(Brushes.Black, blindRegion.X, blindRegion.Y, blindRegion.Width, blindRegion.Height);
                }
                
                graphic.Save();
                image.Save(path, ImageFormat.Png);
            }
        }

        private static string ComputeHash(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }
    }
}