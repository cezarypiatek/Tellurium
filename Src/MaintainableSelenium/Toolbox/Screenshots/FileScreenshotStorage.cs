using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class FileTestStorage : ITestRepository, ITestCaseRepository, IDisposable, ITestSessionRepository
    {
        public class TestStorageModel
        {
            public List<BlindRegion> GlobalBlindRegions { get; set; }
            public Dictionary<string, List<BlindRegion>> CommonBlindRegions { get; set; }
            public List<TestCase> TestCases { get; set; }
            public List<TestResultInfo> TestResults { get; set; }
            public List<TestSessionInfo> TestSessions { get; set; }
            public TestStorageModel()
            {
                TestCases = new List<TestCase>();
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

        public void Save(TestCase testCase)
        {
            StorageModel.TestCases.Add(testCase);
            Persist();
            var screenshotPath = string.Format("{0}\\{1}.png", outputPath, testCase.PatternScreenshot.Hash);
            SaveScreenshot(testCase.PatternScreenshot.Image, screenshotPath);
        }

        public TestCase Find(string testName, string screenshotName, string browserName)
        {
            return StorageModel.TestCases.FirstOrDefault(
                    x => x.TestName == testName && x.PatternScreenshotName == screenshotName && x.BrowserName == browserName);
        }

        public void Save(TestSessionInfo testSession)
        {
            if (string.IsNullOrWhiteSpace(testSession.SessionId))
            {
                testSession.SessionId = IdGenerator.GetNewId();
            }
            if (StorageModel.TestSessions == null)
            {
                StorageModel.TestSessions = new List<TestSessionInfo>();
            }

            if (StorageModel.TestSessions.Any(x => x.SessionId == testSession.SessionId) == false)
            {
                StorageModel.TestSessions.Add(testSession);
            }
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

        public List<ExtendedTestCaseInfo> GetTestCases()
        {
            return this.StorageModel.TestCases.GroupBy(x=>x.PatternScreenshotName).Select(x =>
            {
                return new ExtendedTestCaseInfo
                {
                    Browsers = x.Select(y => new TestCaseShortcut()
                    {
                        BrowserName = y.BrowserName,
                        TestCaseId = y.Id
                    }).ToList(),
                    TestCaseName = x.Key
                };
            }).ToList();
        }

        public List<TestResultInfo> GetTestsFromSession(string sessionId, string browserName)
        {
            return
                this.StorageModel.TestResults.Where(
                    x => x.TestSession.SessionId == sessionId && x.BrowserName == browserName)
                    .ToList();
        }

        public TestCase Get(string testCaseId)
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


        public void MarkAsPattern(string testResultId)
        {
            var testResult = this.StorageModel.TestResults.First(x => x.Id == testResultId);
            var testCase = this.StorageModel.TestCases.First(x => x.Id == testResult.TestCaseId);
            testResult.TestPassed = true;
            testCase.PatternScreenshot.Image = testResult.ErrorScreenshot.Image;
            testCase.PatternScreenshot.Hash= testResult.ErrorScreenshot.Hash;
            Persist();
        }

        public void SaveLocalBlindregions(string testCaseId, List<BlindRegion> localBlindRegions)
        {
            var testCase = this.StorageModel.TestCases.First(x => x.Id == testCaseId);
            testCase.BlindRegions.Clear();
            testCase.BlindRegions.AddRange(localBlindRegions);
            UpdateTestCaseHash(testCase);
            Persist();
        }

        private void UpdateTestCaseHash(TestCase testCase)
        {
            var global = this.GetGlobalBlindRegions(testCase.BrowserName);
            testCase.PatternScreenshot.Hash = ImageHelpers.ComputeHash(testCase.PatternScreenshot.Image, global, testCase.BlindRegions);

        }

        public void SaveGlobalBlindregions(string browserName, List<BlindRegion> globalBlindRegions)
        {
            if (this.StorageModel.CommonBlindRegions == null)
            {
                this.StorageModel.CommonBlindRegions  = new Dictionary<string, List<BlindRegion>>();
            }
            this.StorageModel.CommonBlindRegions[browserName] = globalBlindRegions.ToList();
            this.StorageModel.TestCases
                .Where(x => x.BrowserName == browserName)
                .AsParallel()
                .ForAll(UpdateTestCaseHash);
            Persist();
        }

        public List<BlindRegion> GetGlobalBlindRegions(string browserName)
        {
            if (this.StorageModel.CommonBlindRegions== null || this.StorageModel.CommonBlindRegions.ContainsKey(browserName) == false)
            {
                return new List<BlindRegion>();
            }

            return this.StorageModel.CommonBlindRegions[browserName].ToList();
        }

        public void MarkAllAsPattern(string testSessionId, string browserName)
        {
            var testResults = this.GetTestsFromSession(testSessionId, browserName);
            foreach (var testResult in testResults.Where(x=>x.TestPassed == false))
            {
                var testCase = this.StorageModel.TestCases.First(x => x.Id == testResult.TestCaseId);
                testCase.PatternScreenshot.Image = testResult.ErrorScreenshot.Image;
                testCase.PatternScreenshot.Hash = testResult.ErrorScreenshot.Hash;
            }
            Persist();
        }
    }
}