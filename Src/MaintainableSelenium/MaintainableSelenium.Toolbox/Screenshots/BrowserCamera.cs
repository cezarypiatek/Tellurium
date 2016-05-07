using System;
using System.Collections.Generic;
using MaintainableSelenium.Toolbox.Infrastructure;
using MaintainableSelenium.Toolbox.Infrastructure.Persistence;
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using MaintainableSelenium.Toolbox.Screenshots.Lens;
using MaintainableSelenium.Toolbox.SeleniumUtils;
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
        private readonly IBrowserCameraLens lens;
        private readonly ISet<ScreenshotIdentity> takenScreenshots = new HashSet<ScreenshotIdentity>();


        public string ProjectName { get; set; }
        public string ScreenshotCategory { get; set; }
        public string BrowserName { get; set; }

        public BrowserCamera(RemoteWebDriver driver, ScreenshotService screenshotService, IBrowserCameraLens lens)
        {
            this.driver = driver;
            this.screenshotService = screenshotService;
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
            screenshotService.Persist(screenshot, screenshotIdentity);
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

        public static IBrowserCamera CreateNew(RemoteWebDriver driver, string browserName, string projectName, string screenshotCategory, LensType lensType = LensType.Regular)
        {
            var screenshotService = new ScreenshotService(new Repository<Project>(PersistanceEngine.GetSessionContext()));
            var lens = BrowserCameraLensFactory.Create(driver, lensType);
            return new BrowserCamera(driver, screenshotService, lens)
            {
                ProjectName = projectName,
                ScreenshotCategory = screenshotCategory,
                BrowserName = browserName
            };
        }
    }
}