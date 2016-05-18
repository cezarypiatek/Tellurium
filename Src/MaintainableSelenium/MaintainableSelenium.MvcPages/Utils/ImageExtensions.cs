using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MaintainableSelenium.MvcPages.Utils
{
    public static class ImageExtensions
    {
        public static Bitmap ToBitmap(this byte[] screenshot)
        {
            using (MemoryStream memoryStream = new MemoryStream(screenshot))
            {
                var image = Image.FromStream(memoryStream);
                return new Bitmap(image);
            }
        }

        public static byte[] ToBytes(this Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
    }
}
