using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class UriExtension
    {
        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Image DownloadImage(this Uri uri)
        {
            using var wc = new WebClient();
            using var imgStream = new MemoryStream(wc.DownloadData(uri));
            return Image.FromStream(imgStream);
        }
    }
}