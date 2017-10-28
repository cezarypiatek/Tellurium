using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Tellurium.MvcPages.SeleniumUtils.WebDrivers
{
    public class ChromeDriverExtended : ChromeDriver
    {

        public ChromeDriverExtended(string chromeDriverDirectory) : base(chromeDriverDirectory)
        {
            InitializeSendCommand();
        }

        public ChromeDriverExtended(ChromeDriverService service) : base(service)
        {
            InitializeSendCommand();
        }

        public ChromeDriverExtended(ChromeDriverService service, ChromeOptions options) : base(service, options)
        {
            InitializeSendCommand();
        }

        private void InitializeSendCommand()
        {
            var commandInfo = new CommandInfo(CommandInfo.PostCommand,
                $"/session/{SessionId}/chromium/send_command_and_get_result");
            CommandExecutor.CommandInfoRepository.TryAddCommand("sendCommand", commandInfo);
        }
        

        public new Screenshot GetScreenshot()
        {

            SetDeviceMetricsForFullPageScreenshot();

            try
            {
                var captureScreenshotResponse = SendCommand("Page.captureScreenshot", new object());

                var result = (Dictionary<string, object>) captureScreenshotResponse.Value;
                var base64String = result["data"].ToString();
                return new Screenshot(base64String);
            }
            finally
            {
                ClearDeviceMetrics();
            }
        }

        private void SetDeviceMetricsForFullPageScreenshot()
        {
            var bodySize = FindElementByTagName("body").Size;


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

            return Execute("sendCommand", commandParams);

        }
    }
}
