namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Represents regions ignored by <see cref="BrowserCamera"/> when comparing images
    /// </summary>
    public class BlindRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public BlindRegion()
        {
        }

        public BlindRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}