using Tellurium.MvcPages.BrowserCamera;
using Tellurium.VisualAssertions.Infrastructure;

namespace Tellurium.VisualAssertions.Screenshots.Domain
{
    /// <summary>
    /// Represents regions ignored by <see cref="BrowserCamera"/> when comparing images
    /// </summary>
    public class BlindRegion:Entity
    {
        public virtual float Left { get; set; }
        public virtual float Top { get; set; }
        public virtual float Width { get; set; }
        public virtual float Height { get; set; }

        public BlindRegion()
        {
        }

        public BlindRegion(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}