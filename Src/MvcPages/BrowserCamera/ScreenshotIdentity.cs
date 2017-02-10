namespace Tellurium.MvcPages.BrowserCamera
{
    public class ScreenshotIdentity
    {
        public string ProjectName { get; }
        public string BrowserName { get; }
        public string Category { get; }
        public string ScreenshotName { get; }

        public string FullName => $"{ProjectName} \\ {BrowserName} \\ {Category} \\ {ScreenshotName}";

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
            return $"{ProjectName}_{BrowserName}_{Category}_{ScreenshotName}".GetHashCode();
        }
    }
}