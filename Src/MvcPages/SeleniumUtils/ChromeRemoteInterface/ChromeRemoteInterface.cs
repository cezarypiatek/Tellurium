using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Tellurium.MvcPages.SeleniumUtils.FileUploading;

namespace Tellurium.MvcPages.SeleniumUtils.ChromeRemoteInterface
{
    public class ChromeRemoteInterface
    {
        public static bool IsSupported(RemoteWebDriver driver)
        {
            return driver.Capabilities.BrowserName.Equals("chrome", StringComparison.InvariantCultureIgnoreCase);
        }

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


        private Response TrySendCommand(string cmd, object parameters = null)
        {
            try
            {
                return SendCommand(cmd, parameters);
            }
            catch (TargetInvocationException)
            {
                return null;
            }
        }

        private Response SendCommand(string cmd, object parameters=null)
        {
            var commandParams = new Dictionary<string, object> {{"cmd", cmd}, {"params", parameters ?? new object()}};
            var response = WebDriverCommandExecutor.Execute(driver, "sendCommand", commandParams);
            if (response.Status != WebDriverResult.Success)
            {
                throw new ChromeRemoteInterfaceInvocationException(cmd, parameters, response);
            }
            return response;
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
            SendCommand("Emulation.clearDeviceMetricsOverride");
        }

        public byte[] CaptureScreenshot()
        {
            var captureScreenshotResponse = SendCommand("Page.captureScreenshot");
            var base64String = captureScreenshotResponse.GetValue<string>("data");
            return new Screenshot(base64String).AsByteArray;
        }

        public void SetFileInputFiles(IWebElement inputElement, string[] filePaths)
        {
            SendCommand("DOM.setFileInputFiles", new Dictionary<string, object>
            {
                ["files"] = filePaths.Select(FileUploadingExtensions.GetAbsoluteExistingPath).ToArray(),
                ["nodeId"] = GetChromeNodeId(inputElement)
            });
        }

        private long GetChromeNodeId(IWebElement inputElement)
        {
            driver.ExecuteScript(@"(function(fileInput){
                window.__tellurium_chromerinode = fileInput;
            })(arguments[0])", inputElement);


            var evaluateResponse = SendCommand("Runtime.evaluate", new Dictionary<string, object>
            {
                ["expression"] = "window.__tellurium_chromerinode"
            });

            driver.ExecuteScript(@"(function(fileInput){
                delete window.__tellurium_chromerinode;
            })()");
            
            var remoteObjectId = evaluateResponse.GetValue<string>("result.objectId");
            
            var rquestNodeResponse = SendCommand("DOM.requestNode", new Dictionary<string, object>
            {
                ["objectId"] = remoteObjectId
            });

            return rquestNodeResponse.GetValue<long>("nodeId");
        }

        public void TryEnableFileDownloading(string downloadPath)
        {
            TrySendCommand("Page.setDownloadBehavior", new Dictionary<string, object>()
            {
                ["behavior"] = "allow",
                ["downloadPath"] = downloadPath
            });
        }
    }
}