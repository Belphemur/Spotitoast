using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Gdk;
using Image = System.Drawing.Image;

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

        public static IconData ToIconData(this Pixbuf pixbuf)
        {
            var iconData = new IconData
            {
                Height = pixbuf.Height,
                Width = pixbuf.Width,
                Rowstride = pixbuf.Rowstride,
                HasAlpha = pixbuf.HasAlpha,
                BitsPerSample = pixbuf.BitsPerSample,
                NChannels = pixbuf.NChannels
            };
            var len = (iconData.Height - 1) * iconData.Rowstride + iconData.Width * ((iconData.NChannels * iconData.BitsPerSample + 7) / 8);
            iconData.Pixels = new byte[len];
            Marshal.Copy(pixbuf.Pixels, iconData.Pixels, 0, len);
            return iconData;
        }

        public static Pixbuf ToPixbuf(this Image image)
        {
            using var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            var pixbuf = new Pixbuf(stream);
            return pixbuf;
        }
    }
}