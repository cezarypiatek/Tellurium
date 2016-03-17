using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public static Bitmap CreateImageDiff(Bitmap source, Bitmap overlay, List<BlindRegion> blindRegions=null)
        {
            var filter = new ThresholdedDifference(0) {OverlayImage = source};
            var imageDiff = filter.Apply(overlay);
            DilatationFilter.ApplyInPlace(imageDiff);
            var fixedBitmap = CloneBitmapFormat(imageDiff);
            MarkBlindRegions(fixedBitmap, blindRegions);
            var result = CloneBitmapFormat(overlay);
            DrawBounds(fixedBitmap, result);
            return result;
        }

        /// <summary>
        /// Create copy of bitmap and prevent 'A Graphics object cannot be created from an image that has an indexed pixel format.' issue
        /// </summary>
        private static Bitmap CloneBitmapFormat(Bitmap originalBmp)
        {
            var resultBitmap = new Bitmap(originalBmp.Width, originalBmp.Height);
            using(var g = Graphics.FromImage(resultBitmap))
            {
                g.DrawImage(originalBmp, 0, 0);
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

        public static Bitmap CreateImagesXor(Bitmap bitmapA, Bitmap bitmapB, List<BlindRegion> blindRegions=null)
        {
            var pixelBufferA = GetPixelBuffer(bitmapA);
            var pixelBufferB = GetPixelBuffer(bitmapB);
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


            Bitmap resultBitmap = new Bitmap(bitmapA.Width, bitmapA.Height);
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
        private static IEnumerable<Rectangle> GetBoundingRecangles(Bitmap bitmapWithPoints)
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
                    yield return new Rectangle(minX, minY, maxX - minX, maxY - minY);
                }
            }
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