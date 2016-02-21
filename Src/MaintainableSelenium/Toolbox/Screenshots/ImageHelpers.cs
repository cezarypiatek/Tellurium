using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;


namespace MaintainableSelenium.Toolbox.Screenshots
{
    public static class ImageHelpers
    {
        private static readonly BinaryDilatation3x3 DilatationFilter = new BinaryDilatation3x3();
        private static readonly Pen DiffPen = new Pen(Color.FromArgb(128, Color.Red));

        public static Bitmap CreateImageDiff(string patternPath, string errorPath)
        {
            var pattern = new Bitmap(Image.FromFile(patternPath));
            var error = new Bitmap(Image.FromFile(errorPath));
            var diff = CreateImageDiff(pattern, error);
            return diff;
        }

        public static Bitmap CreateImageDiff(Bitmap source, Bitmap overlay)
        {
            var filter = new ThresholdedDifference(0) {OverlayImage = source};
            var imageDiff = filter.Apply(overlay);
            DilatationFilter.ApplyInPlace(imageDiff);
            var fixedBitmap = CloneBitmapFormat(imageDiff);
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
    }
}