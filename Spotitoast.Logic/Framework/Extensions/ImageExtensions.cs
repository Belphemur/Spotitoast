using IronSoftware.Drawing;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Resize the image to the wanted size
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static AnyBitmap ResizeImage(this AnyBitmap image, int width, int height)
        {
            return new AnyBitmap(image, width, height);
        }
    }
}