using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.WebDriverWrappers;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ChromeFullPageLens : IBrowserCameraLens
    {
        private readonly RemoteWebDriver driver;

        public ChromeFullPageLens(RemoteWebDriver driver)
        {
            Version.TryParse(driver.Capabilities.Version, out var version);
            if (!string.Equals(driver.Capabilities.BrowserName, "chrome", StringComparison.OrdinalIgnoreCase) ||
                !(version.Major >= 59))
                throw new Exception("ChromeFullPageLens works only with Chrome version 59 or higher");

            this.driver = driver;

            InitializeSendCommand();
        }

        public byte[] TakeScreenshot()
        {
            driver.ScrollToY(0);

            SetDeviceMetricsForFullPageScreenshot();

            try
            {
                var captureScreenshotResponse = SendCommand("Page.captureScreenshot", new object());

                var result = (Dictionary<string, object>) captureScreenshotResponse.Value;
                var base64String = result["data"].ToString();
                return new Screenshot(base64String).AsByteArray;
            }
            finally
            {
                ClearDeviceMetrics();
            }
        }

        private void InitializeSendCommand()
        {
            var commandInfo = new CommandInfo(CommandInfo.PostCommand,
                $"/session/{driver.SessionId}/chromium/send_command_and_get_result");

            WebDriverCommandExecutor.TryAddCommand(driver, "sendCommand", commandInfo);
        }

        private void SetDeviceMetricsForFullPageScreenshot()
        {
            var bodySize = driver.FindElementByTagName("body").Size;


            var parameters = new Dictionary<string, object>
            {
                {"width", bodySize.Width},
                {"height", bodySize.Height},
                {"deviceScaleFactor", 1},
                {"mobile", false}
            };
            SendCommand("Emulation.setDeviceMetricsOverride", parameters);
        }

        private void ClearDeviceMetrics()
        {
            SendCommand("Emulation.clearDeviceMetricsOverride", new object());
        }


        private Response SendCommand(string cmd, object parameters)
        {
            var commandParams = new Dictionary<string, object> {{"cmd", cmd}, {"params", parameters}};

            return WebDriverCommandExecutor.Execute(driver, "sendCommand", commandParams);
        }
    }
}