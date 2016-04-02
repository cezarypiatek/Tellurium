namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ScreenshotData: Entity
    {
        public virtual string Hash { get; set; }
        public virtual byte[] Image { get; set; }
    }
}