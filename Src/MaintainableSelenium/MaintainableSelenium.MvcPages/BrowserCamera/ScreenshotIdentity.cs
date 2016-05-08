namespace MaintainableSelenium.MvcPages.BrowserCamera
{
    public class ScreenshotIdentity
    {
        public string ProjectName { get; private set; }
        public string BrowserName { get; private set; }
        public string Category { get; private set; }
        public string ScreenshotName { get; private set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} \\ {1} \\ {2} \\ {3}", ProjectName, BrowserName, Category, ScreenshotName);
            }
        }

        public ScreenshotIdentity(string projectName, string browserName, string category, string screenshotName)
        {
            ProjectName = projectName;
            BrowserName = browserName;
            Category = category;
            ScreenshotName = screenshotName;
        }

        public override bool Equals(object obj)
        {
            var screenshotIdentityObj = obj as ScreenshotIdentity;
            if (screenshotIdentityObj == null)
            {
                return false;
            }
            return this.GetHashCode() == screenshotIdentityObj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}_{3}",ProjectName, BrowserName, Category,ScreenshotName).GetHashCode();
        }
    }
}