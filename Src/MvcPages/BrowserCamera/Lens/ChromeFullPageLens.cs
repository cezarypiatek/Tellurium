using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils;
using Tellurium.MvcPages.SeleniumUtils.WebDriverWrappers;

namespace Tellurium.MvcPages.BrowserCamera.Lens
{
    public class ChromeFullPageLens : IBrowserCameraLens
    {
        private readonly ChromeDriver driver;

        public ChromeFullPageLens(RemoteWebDriver driver)
        {
            this.driver = driver as ChromeDriver ?? throw new Exception("ChromeFullPageLens works only with BrowserType: ChromeDriver");

            InitializeSendCommand();
        }

        public byte[] TakeScreenshot()
        {
            driver.ScrollToY(0);

            SetDeviceMetricsForFullPageScreenshot();

            try
            {
                var captureScreenshotResponse = SendCommand("Page.captureScreenshot", new object());

                var result = (Dictionary<string, object>)captureScreenshotResponse.Value;
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


            var parameters = new Dictionary<string, object>();
            parameters.Add("width", bodySize.Width);
            parameters.Add("height", bodySize.Height);
            parameters.Add("deviceScaleFactor", 1);
            parameters.Add("mobile", false);
            var result = SendCommand("Emulation.setDeviceMetricsOverride", parameters);
        }

        private void ClearDeviceMetrics()
        {
            var result = SendCommand("Emulation.clearDeviceMetricsOverride", new object());
        }


        private Response SendCommand(string cmd, object parameters)
        {
            var commandParams = new Dictionary<string, object>();
            commandParams.Add("cmd", cmd);
            commandParams.Add("params", parameters);

            return WebDriverCommandExecutor.Execute(driver,"sendCommand", commandParams);

        }
    }
}