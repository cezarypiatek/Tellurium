using MaintainableSelenium.Toolbox.Infrastructure;

namespace MaintainableSelenium.Toolbox.Screenshots.Domain
{
    public class ScreenshotData: Entity
    {
        public virtual string Hash { get; set; }
        public virtual byte[] Image { get; set; }
    }
}