using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class UriExtension
    {
        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<Image> DownloadImage(this Uri uri)
        {
            using var wc = new WebClient();
            using var imgStream = new MemoryStream(await wc.DownloadDataTaskAsync(uri));
            return Image.FromStream(imgStream);
        }
    }
}