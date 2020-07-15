using System.Collections.Generic;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    public class TestResult : Entity
    {
        public virtual string ScreenshotName { get; set; }
        public virtual string Category { get; set; }
        public virtual string BrowserName { get; set; }
        public virtual TestResultStatus Status { get; set; }
        public virtual string TestResultMessage { get; set; }
        public virtual BrowserPattern Pattern { get; set; }
        public virtual TestSession TestSession { get; set; }
        public virtual byte[] ErrorScreenshot { get; set; }
        public virtual IList<BlindRegion> BlindRegionsSnapshot { get; set; }

        public TestResult()
        {
            BlindRegionsSnapshot = new List<BlindRegion>();
        }

        public virtual void MarkAsPattern()
        {
            if (this.Pattern.IsActive)
            {
                this.Pattern.Deactivate();
                var blindRegionsCopy = this.Pattern.GetCopyOfOwnBlindRegions();
                this.Pattern.TestCase.AddNewPattern(this.ErrorScreenshot, this.Pattern.BrowserName, blindRegionsCopy);
            }
        }
    }

    public enum TestResultStatus
    {
        Failed=0,
        Passed,
        NewPattern
    }
}