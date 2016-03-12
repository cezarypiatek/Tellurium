using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class FileTestStorage:ITestRepository, IDisposable
    {
        public class TestStorageModel
        {
            public List<TestCaseInfo> TestCases { get; set; }
            public List<TestResultInfo> TestResults { get; set; }

            public TestStorageModel()
            {
                TestCases = new List<TestCaseInfo>();
                TestResults = new List<TestResultInfo>();
            }
        }

        private string outputPath;
        public TestStorageModel StorageModel { get; set; }
        public FileTestStorage(string outputPath)
        {
            SetOutputPath(outputPath);
            LoadStorageModel();
        }

        private void LoadStorageModel()
        {
            var storagePath = GetStorageModelPath();
            if (File.Exists(storagePath))
            {
                var fileContent = File.ReadAllText(storagePath);
                StorageModel = JsonConvert.DeserializeObject<TestStorageModel>(fileContent);
            }
            else
            {
                StorageModel = new TestStorageModel();
            }

        }

        private string GetStorageModelPath()
        {
            return Path.Combine(outputPath, "testdata.json");
        }

        private void SetOutputPath(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            this.outputPath = path;
        }

        public void Save(ScreenshotDescription screenshotDescription)
        {
            var patternPath = string.Format("{0}\\{1}_{2}_PATTERN.png", outputPath, screenshotDescription.TestResultInfo.ScreenshotName, screenshotDescription.TestResultInfo.BrowserName);
            
            if (File.Exists(patternPath))
            {
                var errorPath = string.Format("{0}\\{1}_{2}_ERROR.png", outputPath, screenshotDescription.TestResultInfo.ScreenshotName, screenshotDescription.TestResultInfo.BrowserName);
                SaveScreenshot(screenshotDescription.Screenshot, errorPath);
            }
            else
            {
                SaveScreenshot(screenshotDescription.Screenshot, patternPath);
            }
        }

        private void SaveScreenshot(byte[] screenshot, string path)
        {
            var image = ImageHelpers.ConvertBytesToImage(screenshot);
            image.Save(path, ImageFormat.Png);
        }

        public void SaveTestResult(TestResultInfo testResultInfo)
        {
            StorageModel.TestResults.Add(testResultInfo);
            Persist();
            if (testResultInfo.TestPassed == false)
            {
                var screenshotPath = string.Format("{0}\\{1}.png", outputPath, testResultInfo.ErrorScreenshot.Hash);
                SaveScreenshot(testResultInfo.ErrorScreenshot.Image, screenshotPath);
            }
        }

        public void SaveTestCaseInfo(TestCaseInfo testCaseInfo)
        {
            StorageModel.TestCases.Add(testCaseInfo);
            Persist();
            var screenshotPath = string.Format("{0}\\{1}.png", outputPath, testCaseInfo.PatternScreenhotHash);
            SaveScreenshot(testCaseInfo.PatternScreenshot, screenshotPath);
        }

        public TestCaseInfo GetTestCaseInfo(string testName, string screenshotName, string browserName)
        {
            return StorageModel.TestCases.FirstOrDefault(
                    x => x.TestName == testName && x.PatternScreenshotName == screenshotName && x.BrowserName == browserName);
        }

        public List<ExtendedTestSessionInfo> GetTestSessions()
        {
            return this.StorageModel.TestResults
                .Select(x => x.TestSession)
                .GroupBy(x => x.SessionId)
                .Select(x =>
                {
                    var session = x.First();
                    return new ExtendedTestSessionInfo()
                    {
                        TestSession = session,
                        Browsers = GetBrowsersForSession(session.SessionId)
                    };
                })
                .OrderByDescending(x => x.TestSession.StartDate)
                .ToList();
        }

        public List<TestResultInfo> GetTestsFromSession(string sessionId, string browserName)
        {
            return
                this.StorageModel.TestResults.Where(
                    x => x.TestSession.SessionId == sessionId && x.BrowserName == browserName)
                    .ToList();
        }

        public TestCaseInfo GetTestCase(string testCaseId)
        {
            return this.StorageModel.TestCases.First(x => x.Id == testCaseId);
        }

        public TestResultInfo GetTestResult(string testResultId)
        {
            return this.StorageModel.TestResults.First(x => x.Id == testResultId);
        }

        private List<string> GetBrowsersForSession(string sessionId)
        {
            return this.StorageModel.TestResults.Where(x => x.TestSession.SessionId == sessionId)
                .GroupBy(x => x.BrowserName)
                .Select(x => x.First().BrowserName)
                .ToList();
        }

        public void Dispose()
        {
            Persist();
        }

        private void Persist()
        {
            var storagePath = GetStorageModelPath();
            var fileContent = JsonConvert.SerializeObject(StorageModel);
            File.WriteAllText(storagePath, fileContent);
        }

        public void AddBlindRegion(string testCaseId, BlindRegion blindRegion)
        {
            var testCase = this.StorageModel.TestCases.First(x => x.Id == testCaseId);
            if (testCase.BlindRegions == null)
            {
                testCase.BlindRegions = new List<BlindRegion>();
            }
            testCase.BlindRegions.Add(blindRegion);
            Persist();
        }

        public void MarkAsPattern(string testCaseId, string testResultId)
        {
            var testCase = this.StorageModel.TestCases.First(x => x.Id == testCaseId);
            var testResult = this.StorageModel.TestResults.First(x => x.Id == testResultId);
            testCase.PatternScreenshot = testResult.ErrorScreenshot.Image;
            testCase.PatternScreenhotHash = testResult.ErrorScreenshot.Hash;
            Persist();
        }
    }
}