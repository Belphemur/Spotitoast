using System;
using System.Linq;
using System.Threading.Tasks;
using IronSoftware.Drawing;
using SpotifyAPI.Web;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Logic.Model.Song.Adapter
{
    public class AlbumAdapter(SimpleAlbum album, ImageDownloader imageDownloader) : IAlbum
    {
        private readonly Uri _albumArt = new(album.Images.First().Url);
        private Task<AnyBitmap> _artImage;

        public Task<AnyBitmap> Art => _artImage ??= imageDownloader.DownloadImage(_albumArt);
        public string Name { get; } = album.Name;
        public DateTime ReleaseDate { get; } = album.ReleaseDatePrecision == "year" ? new DateTime(int.Parse(album.ReleaseDate), 1, 1) : DateTime.Parse(album.ReleaseDate);

        public void Dispose()
        {
            if (_artImage is { IsCompletedSuccessfully: true })
            {
                _artImage.Result.Dispose();
            }
        }
    }
}