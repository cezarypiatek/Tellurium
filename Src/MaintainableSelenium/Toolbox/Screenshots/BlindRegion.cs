namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Represents regions ignored by <see cref="BrowserCamera"/> when comparing images
    /// </summary>
    public class BlindRegion
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

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