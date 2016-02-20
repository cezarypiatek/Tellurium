namespace MaintainableSelenium.Toolbox.Screenshots
{
    /// <summary>
    /// Represents regions ignored by <see cref="BrowserCamera"/> when comparing images
    /// </summary>
    public class BlindRegion
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BlindRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}