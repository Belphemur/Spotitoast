using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Spotitoast.Banner.ExtensionMethods
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Resize the image to the wanted size
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ResizeImage(this Image image, Size size)
        {
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(size.Width, size.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}