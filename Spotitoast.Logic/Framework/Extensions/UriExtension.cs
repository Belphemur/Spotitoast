using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class UriExtension
    {
        private static readonly RecyclableMemoryStreamManager _streamManager = new RecyclableMemoryStreamManager();
        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<Image> DownloadImage(this Uri uri)
        {
            using var wc = new WebClient();
            var imgBytes = await wc.DownloadDataTaskAsync(uri);
            using var memoryStream = _streamManager.GetStream("Image", imgBytes, 0, imgBytes.Length);
            return Image.FromStream(memoryStream);
        }
    }
}