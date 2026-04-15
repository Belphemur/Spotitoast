using IronSoftware.Drawing;

namespace Notify.Linux.Extensions
{
    internal static class ImageExtensions
    {
        internal struct IconData
        {
            public int Width;
            public int Height;
            public int Rowstride;
            public bool HasAlpha;
            public int BitsPerSample;
            public int NChannels;
            public byte[] Pixels;
        }

        public static IconData ToIconData(this AnyBitmap image)
        {
            return new IconData
            {
                Height = image.Height,
                Width = image.Width,
                Rowstride = image.Width * 4,
                HasAlpha = true,
                BitsPerSample = 8,
                NChannels = 4,
                Pixels = image.ToRgbaBuffer()
            };
        }

        private static byte[] ToRgbaBuffer(this AnyBitmap image)
        {
            var pixels = new byte[image.Width * image.Height * 4];

            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    var index = (y * image.Width + x) * 4;
                    pixels[index] = color.R;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.B;
                    pixels[index + 3] = color.A;
                }
            }

            return pixels;
        }
    }
}