namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class TestResult: Entity
    {
        public BrowserPattern Pattern { get; set; }
        public TestSession TestSession { get; set; }
        public string TestName { get; set; }
        public string ScreenshotName { get; set; }
        public string BrowserName { get; set; }
        public bool TestPassed { get; set; }
        public ScreenshotData ErrorScreenshot { get; set; }
    }
}