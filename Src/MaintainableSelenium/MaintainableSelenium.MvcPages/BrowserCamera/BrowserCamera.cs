using System;
using System.Collections.Generic;
using MaintainableSelenium.MvcPages.BrowserCamera.Lens;
using MaintainableSelenium.MvcPages.SeleniumUtils;
using MaintainableSelenium.MvcPages.Utils;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    /// <summary>
    /// Responsible for taking screenshots of the page
    /// </summary>
    public class BrowserCamera : IBrowserCamera
    {
        private readonly RemoteWebDriver driver;
        private readonly IScreenshotStorage screenshotStorage;
        private readonly IBrowserCameraLens lens;
        private readonly ISet<ScreenshotIdentity> takenScreenshots = new HashSet<ScreenshotIdentity>();


        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public string BrowserName { get; set; }

        public BrowserCamera(RemoteWebDriver driver, IScreenshotStorage screenshotStorage, IBrowserCameraLens lens)
        {
            this.driver = driver;
            this.screenshotStorage = screenshotStorage;
            this.lens = lens;
        }

        public void TakeScreenshot(string name)
        {
            var screenshotIdentity = new ScreenshotIdentity(ProjectName,BrowserName,ScreenshotCategory, name);
            if (takenScreenshots.Contains(screenshotIdentity))
            {
                throw new DuplicatedScreenshotInSession(screenshotIdentity);
            }

            var screenshot = GetScreenshot();
            takenScreenshots.Add(screenshotIdentity);
            screenshotStorage.Persist(screenshot, screenshotIdentity);
        }

        private byte[] GetScreenshot()
        {
            try
            {
                driver.Blur();
                return this.lens.TakeScreenshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetFullExceptionMessage());
                throw;
            }
        }

        public static IBrowserCamera CreateNew(RemoteWebDriver driver, string browserName, BrowserCameraConfig cameraConfig)
        {
            var lens = BrowserCameraLensFactory.Create(driver, cameraConfig.LensType);
            return new BrowserCamera(driver, cameraConfig.ScreenshotStorage, lens)
            {
                ProjectName = cameraConfig.ProjectName,
                ScreenshotCategory = cameraConfig.ScreenshotCategory,
                BrowserName = browserName
            };
        }
    }
}