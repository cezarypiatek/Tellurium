namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotDescription
    {
        public byte[] Screenshot { get; set; }
        public string ScreenshotHash { get; set; }

        public TestResultInfo TestResultInfo { get; set; }
    }
}