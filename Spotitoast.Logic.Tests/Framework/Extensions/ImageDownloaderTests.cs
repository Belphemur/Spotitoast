using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Logic.Tests.Framework.Extensions;

public class ImageDownloaderTests
{
    private static readonly byte[] ImageBytes =
        Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVQIHWP4////fwAJ+wP9KobjigAAAABJRU5ErkJggg==");

    [Fact]
    public async Task DownloadImage_AfterMoreThanTenDistinctImages_CacheCountIsBoundedToTen()
    {
        var handler = new CountingImageHandler();
        var downloader = CreateDownloader(handler);

        for (var index = 0; index < 12; index++)
        {
            await downloader.DownloadImage(CreateUri(index));
        }

        Assert.Equal(12, handler.TotalRequestCount);
        Assert.Equal(10, GetCacheCount(downloader));
    }

    [Fact]
    public async Task DownloadImage_WhenCacheOverflows_EvictsLeastRecentlyUsedImage()
    {
        var handler = new CountingImageHandler();
        var downloader = CreateDownloader(handler);
        var uris = Enumerable.Range(0, 11).Select(CreateUri).ToArray();

        foreach (var uri in uris.Take(10))
        {
            await downloader.DownloadImage(uri);
        }

        await downloader.DownloadImage(uris[0]);
        await downloader.DownloadImage(uris[10]);
        await downloader.DownloadImage(uris[0]);
        await downloader.DownloadImage(uris[1]);

        Assert.Equal(1, handler.GetRequestCount(uris[0]));
        Assert.Equal(2, handler.GetRequestCount(uris[1]));
        Assert.Equal(10, GetCacheCount(downloader));
    }

    private static ImageDownloader CreateDownloader(CountingImageHandler handler)
    {
        return new ImageDownloader(new FakeHttpClientFactory(handler), NullLogger<ImageDownloader>.Instance);
    }

    private static Uri CreateUri(int index)
    {
        return new Uri($"https://example.test/images/{index}.png");
    }

    private static int GetCacheCount(ImageDownloader downloader)
    {
        var field = typeof(ImageDownloader).GetField("_imageCache", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        var cache = field.GetValue(downloader);
        Assert.NotNull(cache);

        var countProperty = cache.GetType().GetProperty("Count");
        Assert.NotNull(countProperty);

        return Assert.IsType<int>(countProperty.GetValue(cache));
    }

    private sealed class FakeHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler = handler;

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }

    private sealed class CountingImageHandler : HttpMessageHandler
    {
        private readonly ConcurrentDictionary<Uri, int> _requestCounts = new();

        public int TotalRequestCount => _requestCounts.Values.Sum();

        public int GetRequestCount(Uri uri)
        {
            return _requestCounts.TryGetValue(uri, out var count) ? count : 0;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri ?? throw new InvalidOperationException("Request URI is required.");

            _requestCounts.AddOrUpdate(uri, 1, (_, count) => count + 1);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ImageBytes)
            });
        }
    }
}