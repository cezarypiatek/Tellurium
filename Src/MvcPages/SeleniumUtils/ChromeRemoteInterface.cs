using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils.WebDriverWrappers;

namespace Tellurium.MvcPages.SeleniumUtils
{
    public class ChromeRemoteInterface
    {
        private readonly RemoteWebDriver driver;

        public ChromeRemoteInterface(RemoteWebDriver driver)
        {
            this.driver = driver;
            InitializeSendCommand();
        }

        void InitializeSendCommand()
        {
            var commandInfo = new CommandInfo(CommandInfo.PostCommand,
                $"/session/{driver.SessionId}/chromium/send_command_and_get_result");
            WebDriverCommandExecutor.TryAddCommand(driver, "sendCommand", commandInfo);
        }

        private Response SendCommand(string cmd, object parameters)
        {
            var commandParams = new Dictionary<string, object> {{"cmd", cmd}, {"params", parameters}};
            return WebDriverCommandExecutor.Execute(driver, "sendCommand", commandParams);
        }

        public void SetDeviceMetricsForFullPage()
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

        public void ClearDeviceMetrics()
        {
            SendCommand("Emulation.clearDeviceMetricsOverride", new object());
        }

        public byte[] CaptureScreenshot()
        {
            var captureScreenshotResponse = SendCommand("Page.captureScreenshot", new object());
            var result = (Dictionary<string, object>) captureScreenshotResponse.Value;
            var base64String = result["data"].ToString();
            return new Screenshot(base64String).AsByteArray;
        }
    }
}