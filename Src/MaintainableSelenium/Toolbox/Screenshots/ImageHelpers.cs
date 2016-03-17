using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;


namespace MaintainableSelenium.Toolbox.Screenshots
{
    public class ImageDiff
    {
        public Bitmap WithBounds { get; set; }
        public Bitmap WithXOR { get; set; }
    }

    public static class ImageHelpers
    {
        private static readonly BinaryDilatation3x3 DilatationFilter = new BinaryDilatation3x3();
        private static readonly Pen DiffPen = new Pen(Color.FromArgb(128, Color.Red));

        //public static ImageDiff CreateImageDiff(string patternPath, string errorPath)
        //{
        //    var pattern = new Bitmap(Image.FromFile(patternPath));
        //    var error = new Bitmap(Image.FromFile(errorPath));
        //    var diff = CreateImageDiff(pattern, error);
        //    return new ImageDiff()
        //    {
        //        WithBounds = diff,
        //        WithXOR = CreateImagesXor(pattern, error)
        //    };
        //}

        public static Bitmap CreateImageDiff(Bitmap a, Bitmap b, List<BlindRegion> blindRegions=null)
        {
            var unified = UnifiImagesDimensions(a, b);
            var filter = new ThresholdedDifference(0) {OverlayImage = unified.Item1};
            var imageDiff = filter.Apply(unified.Item2);
            DilatationFilter.ApplyInPlace(imageDiff);
            var fixedBitmap = CloneBitmapFormat(imageDiff);
            MarkBlindRegions(fixedBitmap, blindRegions);
            var result = CloneBitmapFormat(unified.Item2);
            DrawBounds(fixedBitmap, result);
            return result;
        }

        private static Tuple<Bitmap, Bitmap> UnifiImagesDimensions(Bitmap a, Bitmap b)
        {
            if (a.Width == b.Width && a.Height == b.Height)
            {
                return new Tuple<Bitmap, Bitmap>(a,b);
            }

            if (a.Height >= b.Height && a.Width >= b.Width)
            {
                return new Tuple<Bitmap, Bitmap>(a, RedrawOnCanvas(b, a.Width, a.Height));
            }

            if (b.Height >= a.Height && b.Width >= a.Width)
            {
                return new Tuple<Bitmap, Bitmap>(RedrawOnCanvas(a, b.Width, b.Height), b);
            }

            var maxWidth = Math.Max(a.Width, b.Width);
            var maxHeight = Math.Max(a.Height, b.Height);
            return new Tuple<Bitmap, Bitmap>(RedrawOnCanvas(a, maxWidth, maxHeight), RedrawOnCanvas(b, maxWidth, maxHeight));
        }

        /// <summary>
        /// Create copy of bitmap and prevent 'A Graphics object cannot be created from an image that has an indexed pixel format.' issue
        /// </summary>
        private static Bitmap CloneBitmapFormat(Bitmap originalBmp)
        {
            return RedrawOnCanvas(originalBmp, originalBmp.Width, originalBmp.Height);
        }

        private static Bitmap RedrawOnCanvas(Bitmap bitmapToRedraw, int canvasWidth, int canvasHeight)
        {
            var resultBitmap = new Bitmap(canvasWidth, canvasHeight);
            using (var g = Graphics.FromImage(resultBitmap))
            {
                g.DrawImage(bitmapToRedraw, 0, 0);
            }
            return resultBitmap;
        }

        /// <summary>
        /// Draw rectangles surrounding point clumps
        /// </summary>
        private static void DrawBounds(Bitmap bitmapWithPoints, Bitmap bitmapToDrawOverlay)
        {
            using (var resultGraphics = Graphics.FromImage(bitmapToDrawOverlay))
            {
                foreach (var boundingRecangle in GetBoundingRecangles(bitmapWithPoints))
                {
                    resultGraphics.DrawRectangle(DiffPen, boundingRecangle);
                }
            }
        }

        public static Bitmap CreateImagesXor(Bitmap a, Bitmap b, List<BlindRegion> blindRegions=null)
        {
            var unified = UnifiImagesDimensions(a, b);
            var pixelBufferA = GetPixelBuffer(unified.Item1);
            var pixelBufferB = GetPixelBuffer(unified.Item2);
            var resultBuffer = new byte[pixelBufferB.Length];
            Array.Copy(pixelBufferB, resultBuffer, pixelBufferA.Length);
            int blue = 0, green = 0, red = 0;


            for (int k = 0; k + 4 < pixelBufferA.Length; k += 4)
            {
                blue = pixelBufferA[k] ^ pixelBufferB[k];
                green = pixelBufferA[k + 1] ^ pixelBufferB[k + 1];
                red = pixelBufferA[k + 2] ^ pixelBufferB[k + 2];

                if (blue < 0)
                { blue = 0; }
                else if (blue > 255)
                { blue = 255; }

                if (green < 0)
                { green = 0; }
                else if (green > 255)
                { green = 255; }


                if (red < 0)
                { red = 0; }
                else if (red > 255)
                { red = 255; }

                resultBuffer[k] = (byte)blue;
                resultBuffer[k + 1] = (byte)green;
                resultBuffer[k + 2] = (byte)red;
            }


            Bitmap resultBitmap = new Bitmap(unified.Item1.Width, unified.Item1.Height);
            var lockBoundRectangle = new Rectangle(0, 0,resultBitmap.Width, resultBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(lockBoundRectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            MarkBlindRegions(resultBitmap, blindRegions);

            return resultBitmap;
        }

        private static byte[] GetPixelBuffer(Bitmap sourceBitmap)
        {
            var lockBoundRectangle = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            BitmapData sourceData = sourceBitmap.LockBits(lockBoundRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] pixelBuffer = new byte[sourceData.Stride*sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            return pixelBuffer;
        }

        /// <summary>
        /// Get rectangles surrounding point clumps
        /// </summary>
        private static List<Rectangle> GetBoundingRecangles(Bitmap bitmapWithPoints)
        {
            var bitmapData = bitmapWithPoints.LockBits(
                new Rectangle(0, 0, bitmapWithPoints.Width, bitmapWithPoints.Height),
                ImageLockMode.ReadWrite, bitmapWithPoints.PixelFormat);


            var blobCounter = new BlobCounter
            {
                FilterBlobs = true,
                MinHeight = 5,
                MinWidth = 5
            };


            blobCounter.ProcessImage(bitmapData);
            var blobs = blobCounter.GetObjectsInformation();
            bitmapWithPoints.UnlockBits(bitmapData);
            var result = new List<Rectangle>();
            for (int i = 0; i < blobs.Length; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                if (edgePoints.Count > 0)
                {
                    int minX = edgePoints[0].X, minY = edgePoints[0].Y, maxX = edgePoints[0].X, maxY = edgePoints[0].Y;

                    for (int j = 0; j < edgePoints.Count; j++)
                    {
                        var p = edgePoints[j];
                        if (p.X < minX)
                        {
                            minX = p.X;
                        }
                        else if (p.X > maxX)
                        {
                            maxX = p.X;
                        }

                        if (p.Y < minY)
                        {
                            minY = p.Y;
                        }
                        else if (p.Y > maxY)
                        {
                            maxY = p.Y;
                        }
                    }
                    result.Add(new Rectangle(minX, minY, maxX - minX, maxY - minY));
                }
            }
            var  toRemove = result.Where(rectangle => result.Any(r => r!= rectangle && IsSquareInside(rectangle, r))).ToList();

            foreach (var rectangle in toRemove)
            {
                result.Remove(rectangle);
            }

            return result;
        }

        private static bool IsSquareInside(Rectangle rectangle, Rectangle r)
        {
            return rectangle.Left >= r.Left && rectangle.Right <= r.Right && rectangle.Top >= r.Top && rectangle.Bottom <= r.Bottom;
        }

        public static Image ConvertBytesToImage(byte[] screenshot)
        {
            using (MemoryStream memoryStream = new MemoryStream(screenshot))
            {
                return Image.FromStream(memoryStream);
            }
        }

        public static Bitmap ConvertBytesToBitmap(byte[] screenshot)
        {
            var image = ConvertBytesToImage(screenshot);
            return new Bitmap(image);
        }

        public static byte[] ConvertImageToBytes(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }


        public static void MarkBlindRegions(Image image, List<BlindRegion> blindRegions)
        {
            if (blindRegions == null)
                return;
            
            var graphic = Graphics.FromImage(image);
            foreach (var blindRegion in blindRegions)
            {
                graphic.FillRectangle(Brushes.Black, blindRegion.Left, blindRegion.Top, blindRegion.Width, blindRegion.Height);
            }

            graphic.Save();
        }

        public static string ComputeHash(byte[] screenshot, List<BlindRegion> blindRegions = null)
        {
            var image = ImageHelpers.ConvertBytesToImage(screenshot);
            if (blindRegions != null)
            {
                ImageHelpers.MarkBlindRegions(image, blindRegions);
            }

            var imageBytes = ImageHelpers.ConvertImageToBytes(image);
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(imageBytes)).Replace("-", "");
            }
        }
    }
}