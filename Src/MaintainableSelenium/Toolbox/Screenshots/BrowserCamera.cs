using System;
using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using OpenQA.Selenium.Remote;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Responsible for taking screenshots of the page
    /// </summary>
    public class BrowserCamera : IBrowserCamera
    {
        private readonly RemoteWebDriver driver;
        private readonly ScreenshotService screenshotService;
        private readonly ISet<string> takenScreenshots = new HashSet<string>();


        public string ProjectName { get; set; }
        public string ImageNamePrefix { get; set; }

        public BrowserCamera(RemoteWebDriver driver,  ScreenshotService screenshotService)
        {
            this.driver = driver;
            this.screenshotService = screenshotService;
        }

        public void TakeScreenshot(string name)
        {
            driver.Blur();
            var screenshot = driver.GetScreenshot();
            var fullName = string.Format("{0}_{1}", ImageNamePrefix, name);
            var screenshotUniqueName = string.Format("{0}_{1}", ProjectName, fullName);
            if (takenScreenshots.Contains(screenshotUniqueName))
            {
                throw new DuplicatedScreenshotInSession(fullName);
            }
            takenScreenshots.Add(screenshotUniqueName);
            screenshotService.Persist(fullName, driver.Capabilities.BrowserName, screenshot.AsByteArray, ProjectName);
        }

        public static IBrowserCamera CreateNew(RemoteWebDriver driver,  string projectName, string imageNamePrefix)
        {
            var screenshotService = new ScreenshotService(new Repository<Project>());
            return new BrowserCamera(driver, screenshotService)
            {
                ProjectName = projectName,
                ImageNamePrefix = imageNamePrefix
            };
        }
    }

    public class DuplicatedScreenshotInSession: ApplicationException
    {

        public DuplicatedScreenshotInSession(string screenshotName)
            :base(string.Format("Cannot take twice the same screenshot. Duplicated screenshot name {0}", screenshotName))
        {
        }
    }
}