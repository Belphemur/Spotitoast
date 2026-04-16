using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IronSoftware.Drawing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Spotitoast.Logic.Framework.Extensions
{
    public class ImageDownloader
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageDownloader> _logger;
        private readonly ConcurrentDictionary<Uri, Lazy<Task<AnyBitmap>>> _inFlightDownloads = new();

        public ImageDownloader(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<ImageDownloader> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Download in memory the image and return it as object.
        /// Concurrent calls for the same URI are coalesced so only one HTTP request is made.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Task<AnyBitmap> DownloadImage(Uri uri)
        {
            if (_memoryCache.TryGetValue(uri, out byte[] imageBytes))
            {
                return Task.FromResult(AnyBitmap.FromBytes(imageBytes));
            }

            var lazy = _inFlightDownloads.GetOrAdd(uri,
                u => new Lazy<Task<AnyBitmap>>(() => DownloadImageCore(u), LazyThreadSafetyMode.ExecutionAndPublication));
            return lazy.Value;
        }

        private async Task<AnyBitmap> DownloadImageCore(Uri uri)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("ImageDownloader");
                using var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                using var entry = _memoryCache.CreateEntry(uri);
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                entry.Value = imageBytes;

                return AnyBitmap.FromBytes(imageBytes);
            }
            catch (HttpRequestException e)
            {
                _logger.LogWarning(e, "Failed to download image from {Uri}", uri);
                return new AnyBitmap(15, 15);
            }
            finally
            {
                _inFlightDownloads.TryRemove(uri, out _);
            }
        }
    }
}