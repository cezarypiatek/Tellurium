using System;
using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Infrastructure.Persistence;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using MaintainableSelenium.Toolbox.SeleniumUtils;
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
        private readonly ScreenshotService screenshotService;
        private readonly ISet<ScreenshotIdentity> takenScreenshots = new HashSet<ScreenshotIdentity>();


        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public string BrowserName { get; set; }

        public BrowserCamera(RemoteWebDriver driver,  ScreenshotService screenshotService)
        {
            this.driver = driver;
            this.screenshotService = screenshotService;
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
            screenshotService.Persist(screenshot.AsByteArray, screenshotIdentity);
        }

        private Screenshot GetScreenshot()
        {
            try
            {
                driver.Blur();
                return driver.GetScreenshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetFullExceptionMessage());
                throw;
            }
        }

        public static IBrowserCamera CreateNew(RemoteWebDriver driver, string browserName, string projectName, string screenshotCategory)
        {
            var screenshotService = new ScreenshotService(new Repository<Project>(PersistanceEngine.GetSessionContext()));
            return new BrowserCamera(driver, screenshotService)
            {
                ProjectName = projectName,
                ScreenshotCategory = screenshotCategory,
                BrowserName = browserName
            };
        }
    }
}