using System;
using System.Net.Http;
using System.Threading.Tasks;
using IronSoftware.Drawing;
using Microsoft.Extensions.Caching.Memory;

namespace Spotitoast.Logic.Framework.Extensions
{
    public class ImageDownloader
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageDownloader(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Download in memory the image and return it as object
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<AnyBitmap> DownloadImage(Uri uri)
        {
            if (_memoryCache.TryGetValue(uri, out byte[] imageBytes))
            {
                return AnyBitmap.FromBytes(imageBytes);
            }

            try
            {
                using var entry = _memoryCache.CreateEntry(uri);
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                using var client = _httpClientFactory.CreateClient("ImageDownloader");
                using var response = await client.GetAsync(uri);
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