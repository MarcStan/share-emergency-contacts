using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconGenerator
{
    /// <summary>
    /// Provides various image untilities, such as high quality resizing and the ability to save a JPEG.
    /// </summary>
    /// <remarks>
    /// From https://stackoverflow.com/a/353222
    /// </remarks>
    public static class ImageUtilities
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="file">The image to resize.</param>
        /// <param name="target">The output path.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="maintainAspectRatio">If true, the image is centered in the output and keeps the source aspect ratio.</param>
        /// <returns>The resized image.</returns>
        public static void ResizeImage(string file, string target, int width, int height, bool maintainAspectRatio)
        {
            var image = Image.FromFile(file);
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //draw the image into the target bitmap
                if (maintainAspectRatio)
                {
                    var minSize = Math.Min(result.Height, result.Width);
                    var sourceAspect = (float)Math.Max(image.Width, image.Height) / Math.Min(image.Width, image.Height);
                    var maxSize = minSize * sourceAspect;
                    var xSize = sourceAspect >= 1 ? maxSize : minSize;
                    var ySize = sourceAspect >= 1 ? minSize : maxSize;
                    var offsetX = (result.Width - xSize) / 2;
                    var offsetY = (result.Height - ySize) / 2;
                    graphics.DrawImage(image, offsetX, offsetY, xSize, ySize);
                }
                else
                {
                    graphics.DrawImage(image, 0, 0, result.Width, result.Height);
                }

            }
            var dir = new FileInfo(target).DirectoryName;
            Directory.CreateDirectory(dir);
            result.Save(target, ImageFormat.Png);
        }
    }
}