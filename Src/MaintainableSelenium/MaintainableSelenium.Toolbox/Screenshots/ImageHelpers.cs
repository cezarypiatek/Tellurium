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
using MaintainableSelenium.Toolbox.Screenshots.Domain;
using Image = System.Drawing.Image;


namespace MaintainableSelenium.Toolbox.Screenshots
{
    public static class ImageHelpers
    {
        private static readonly BinaryDilatation3x3 DilatationFilter = new BinaryDilatation3x3();
        private static readonly Pen DiffPen = new Pen(Color.FromArgb(128, Color.Red));

        public static Bitmap CreateImageDiff(Bitmap a, Bitmap b, IList<BlindRegion> globalBlindRegions, IList<BlindRegion> localBlindRegions)
        {
            var unified = UnifiImagesDimensions(a, b);
            var filter = new ThresholdedDifference(0) {OverlayImage = unified.Item1};
            var imageDiff = filter.Apply(unified.Item2);
            DilatationFilter.ApplyInPlace(imageDiff);
            var fixedBitmap = CloneBitmapFormat(imageDiff);
            MarkBlindRegions(fixedBitmap, globalBlindRegions);
            MarkBlindRegions(fixedBitmap, localBlindRegions);
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

        /// <summary>
        /// Create copy of bitmap with given dimensions
        /// </summary>
        /// <param name="bitmapToRedraw">Bitmap to copt</param>
        /// <param name="canvasWidth">Max width</param>
        /// <param name="canvasHeight">Max Height</param>
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

        /// <summary>
        /// Create a bitmap which represents XOR of two other bitmaps
        /// </summary>
        /// <param name="a">Bitmap a</param>
        /// <param name="b">Bitmap b</param>
        /// <param name="globalBlindRegions">List of squares to ignore</param>
        public static Bitmap CreateImagesXor(Bitmap a, Bitmap b, IList<BlindRegion> globalBlindRegions, IList<BlindRegion> localBlindRegions)
        {
            var unified = UnifiImagesDimensions(a, b);
            var pixelBufferA = GetPixelBuffer(unified.Item1);
            var pixelBufferB = GetPixelBuffer(unified.Item2);
            var resultBuffer = new byte[pixelBufferB.Length];
            Array.Copy(pixelBufferB, resultBuffer, pixelBufferA.Length);

            for (int k = 0; k + 4 < pixelBufferA.Length; k += 4)
            {
                var blue = pixelBufferA[k] ^ pixelBufferB[k];
                var green = pixelBufferA[k + 1] ^ pixelBufferB[k + 1];
                var red = pixelBufferA[k + 2] ^ pixelBufferB[k + 2];

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


            var resultBitmap = new Bitmap(unified.Item1.Width, unified.Item1.Height);
            var lockBoundRectangle = new Rectangle(0, 0,resultBitmap.Width, resultBitmap.Height);
            var resultData = resultBitmap.LockBits(lockBoundRectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            MarkBlindRegions(resultBitmap, globalBlindRegions);
            MarkBlindRegions(resultBitmap, localBlindRegions);

            return resultBitmap;
        }

        private static byte[] GetPixelBuffer(Bitmap sourceBitmap)
        {
            var lockBoundRectangle = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            var sourceData = sourceBitmap.LockBits(lockBoundRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var pixelBuffer = new byte[sourceData.Stride*sourceData.Height];
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
                var edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                if (edgePoints.Count > 0)
                {
                    var surroundingSquare = GetSurroundingSquare(edgePoints);
                    result.Add(surroundingSquare);
                }
            }
            return RemoveNestedRectangles(result);
        }

        private static Rectangle GetSurroundingSquare(List<IntPoint> edgePoints)
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
            var surroundingSquare = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            return surroundingSquare;
        }

        private static List<Rectangle> RemoveNestedRectangles(List<Rectangle> result)
        {
            var toRemove = result.Where(rectangle => result.Any(r => r != rectangle && IsRectangleInside(rectangle, r))).ToList();

            foreach (var rectangle in toRemove)
            {
                result.Remove(rectangle);
            }

            return result;
        }

        private static bool IsRectangleInside(Rectangle rectangle, Rectangle container)
        {
            return rectangle.Left >= container.Left 
                &&  rectangle.Right <= container.Right 
                && rectangle.Top >= container.Top 
                && rectangle.Bottom <= container.Bottom;
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

        private static byte[] ConvertImageToBytes(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static void MarkBlindRegions(Image image, IList<BlindRegion> blindRegions)
        {
            if (blindRegions == null || blindRegions.Count == 0)
                return;
            
            var graphic = Graphics.FromImage(image);
            foreach (var blindRegion in blindRegions)
            {
                graphic.FillRectangle(Brushes.Black, blindRegion.Left, blindRegion.Top, blindRegion.Width, blindRegion.Height);
            }

            graphic.Save();
        }

        /// <summary>
        /// Calculate image hash ignoring given regions
        /// </summary>
        /// <param name="screenshot">Source image for hash</param>
        /// <param name="globalBlindRegions">Global Regions to ignore</param>
        /// <param name="localBlindRegions">Regions to ignore</param>
        public static string ComputeHash(byte[] screenshot, IList<BlindRegion> globalBlindRegions, IList<BlindRegion> localBlindRegions=null)
        {
            var image = ConvertBytesToImage(screenshot);

            MarkBlindRegions(image, globalBlindRegions);
            MarkBlindRegions(image, localBlindRegions);

            var imageBytes = ConvertImageToBytes(image);
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(imageBytes)).Replace("-", "");
            }
        }
    }
}