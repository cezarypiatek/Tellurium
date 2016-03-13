namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Represents regions ignored by <see cref="BrowserCamera"/> when comparing images
    /// </summary>
    public class BlindRegion
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public BlindRegion()
        {
        }

        public BlindRegion(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}