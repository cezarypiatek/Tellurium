using Tellurium.VisualAssertions.Screenshots.Domain;

namespace Tellurium.VisualAssertions.Screenshots.Service
{
    public class Screenshot
    {
        public ScreenshotIdentity Identity { get; set; }
        public byte[] Data { get; set; }
    }
}