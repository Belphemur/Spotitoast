using System;
using System.Linq;
using System.Threading.Tasks;
using IronSoftware.Drawing;
using SpotifyAPI.Web;
using Spotitoast.Logic.Framework.Extensions;

namespace Spotitoast.Logic.Model.Song.Adapter
{
    public class AlbumAdapter : IAlbum
    {
        private readonly Uri _albumArt;
        private readonly ImageDownloader _imageDownloader;
        private Task<AnyBitmap> _artImage;

        public Task<AnyBitmap> Art => _artImage ??= _imageDownloader.DownloadImage(_albumArt);
        public string Name { get; }
        public DateTime ReleaseDate { get; }

        public AlbumAdapter(SimpleAlbum album, ImageDownloader imageDownloader)
        {
            _imageDownloader = imageDownloader;
            Name = album.Name;
            ReleaseDate = album.ReleaseDatePrecision == "year" ? new DateTime(int.Parse(album.ReleaseDate), 1, 1) : DateTime.Parse(album.ReleaseDate);

            _albumArt = new Uri(album.Images.First().Url);
        }

        public void Dispose()
        {
            if (_artImage is { IsCompletedSuccessfully: true })
            {
                _artImage.Result.Dispose();
            }
        }
    }
}