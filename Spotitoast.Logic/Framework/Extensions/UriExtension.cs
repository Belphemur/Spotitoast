using System;
using System.Net.Http;
using System.Threading.Tasks;
using IronSoftware.Drawing;
using Microsoft.Extensions.Caching.Memory;

namespace Spotitoast.Logic.Framework.Extensions
{
    public static class UriExtension
    {
        private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
        private static readonly HttpClient Client = new();

        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<AnyBitmap> DownloadImage(this Uri uri)
        {
            if (MemoryCache.TryGetValue(uri, out byte[] imageBytes))
            {
                return AnyBitmap.FromBytes(imageBytes);
            }

            try
            {
                using var entry = MemoryCache.CreateEntry(uri);
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                using var response = await Client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                imageBytes = await response.Content.ReadAsByteArrayAsync();
                entry.Value = imageBytes;
                return AnyBitmap.FromBytes(imageBytes);
            }
            catch (HttpRequestException e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
                return new AnyBitmap(15, 15);
            }
        }
    }
}