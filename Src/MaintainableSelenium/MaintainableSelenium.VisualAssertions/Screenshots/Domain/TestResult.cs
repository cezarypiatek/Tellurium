using MaintainableSelenium.VisualAssertions.Infrastructure;

namespace MaintainableSelenium.VisualAssertions.Screenshots.Domain
{
    public class TestResult: Entity
    {
        public virtual string ScreenshotName { get; set; }
        public virtual string Category { get; set; }
        public virtual string BrowserName { get; set; }
        public virtual bool TestPassed { get; set; }
        public virtual BrowserPattern Pattern { get; set; }
        public virtual TestSession TestSession { get; set; }
        public virtual byte[] ErrorScreenshot { get; set; }

        public virtual void MarkAsPattern()
        {
            if (this.Pattern.IsActive)
            {
                this.Pattern.Deactivate();
                this.Pattern.TestCase.AddNewPattern(this.ErrorScreenshot, this.Pattern.BrowserName);
            }
        }
    }
}