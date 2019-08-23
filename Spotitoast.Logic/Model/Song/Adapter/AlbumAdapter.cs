using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;
using Spotitoast.Logic.Framework.Extensions;
using Image = System.Drawing.Image;

namespace Spotitoast.Logic.Model.Song.Adapter
{
    public class AlbumAdapter : IAlbum
    {
        private readonly Uri _albumArt;
        private Task<Image> _artImage;

        public Task<Image> Art => _artImage ??= _albumArt.DownloadImage();
        public string Name { get; }
        public DateTime ReleaseDate { get; }

        public AlbumAdapter(SimpleAlbum album)
        {
            Name = album.Name;
            ReleaseDate = album.ReleaseDatePrecision == "year" ? new DateTime(int.Parse(album.ReleaseDate), 1, 1) : DateTime.Parse(album.ReleaseDate);

            _albumArt = new Uri(album.Images.First().Url);
        }

        public void Dispose()
        {
            _artImage?.Dispose();
        }
    }
}