namespace Tellurium.MvcPages.BrowserCamera
{
    public interface IBrowserCamera
    {
        /// <summary>
        /// Returns byte[] containing screenshot depicting current application view
        /// </summary>
        byte[] TakeScreenshot();
    }
}