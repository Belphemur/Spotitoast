using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BitFaster.Caching.Lru;
using IronSoftware.Drawing;
using Microsoft.Extensions.Logging;

namespace Spotitoast.Logic.Framework.Extensions
{
    public class ImageDownloader(
        IHttpClientFactory httpClientFactory,
        ILogger<ImageDownloader> logger)
    {
        private const int ImageCacheCapacity = 10;
        private static readonly TimeSpan ImageCacheDuration = TimeSpan.FromHours(1);

        private readonly ClassicLru<Uri, CachedImage> _imageCache = new(ImageCacheCapacity);
        private readonly ConcurrentDictionary<Uri, Lazy<Task<AnyBitmap>>> _inFlightDownloads = new();

        /// <summary>
        /// Download in memory the image and return it as object.
        /// Concurrent calls for the same URI are coalesced so only one HTTP request is made.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Task<AnyBitmap> DownloadImage(Uri uri)
        {
            if (_imageCache.TryGet(uri, out var cachedImage))
            {
                if (!cachedImage.IsExpired)
                {
                    return Task.FromResult(AnyBitmap.FromBytes(cachedImage.Bytes));
                }

                _imageCache.TryRemove(uri);
            }

            var lazy = _inFlightDownloads.GetOrAdd(uri,
                u => new Lazy<Task<AnyBitmap>>(() => DownloadImageCore(u), LazyThreadSafetyMode.ExecutionAndPublication));
            return lazy.Value;
        }

        private async Task<AnyBitmap> DownloadImageCore(Uri uri)
        {
            try
            {
                using var client = httpClientFactory.CreateClient("ImageDownloader");
                using var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                _imageCache.AddOrUpdate(uri, new CachedImage(imageBytes, DateTimeOffset.UtcNow.Add(ImageCacheDuration)));

                return AnyBitmap.FromBytes(imageBytes);
            }
            catch (HttpRequestException e)
            {
                logger.LogWarning(e, "Failed to download image from {Uri}", uri);
                return new AnyBitmap(15, 15);
            }
            finally
            {
                _inFlightDownloads.TryRemove(uri, out _);
            }
        }

        private sealed record CachedImage(byte[] Bytes, DateTimeOffset ExpiresAtUtc)
        {
            public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
        }
    }
}