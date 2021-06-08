using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class UriExtension
    {
        private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<Image> DownloadImage(this Uri uri)
        {
            if (MemoryCache.TryGetValue(uri, out Image image))
            {
                return image;
            }

            try
            {
                using var entry = MemoryCache.CreateEntry(uri);
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                using var client = new HttpClient();
                var response = await client.GetAsync(uri);
                var contentStream = await response.Content.ReadAsStreamAsync();
                image = Image.FromStream(contentStream);
                entry.Value = image;
                return image;
            }
            catch (HttpRequestException e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
                return new Bitmap(15, 15);
            }
        }
    }
}