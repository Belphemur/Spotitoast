using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyAPI.Web.Models;
using Spotitoast.Logic.Framework.Extensions;
using Image = System.Drawing.Image;

namespace Spotitoast.Logic.Model.Song.Adapter
{
    public class AlbumAdapter : IAlbum
    {
        private readonly Uri _albumArt;
        private Image _artImage;

        public Image Art => _artImage ??= _albumArt.DownloadImage();
        public string Name { get; }
        public DateTime ReleaseDate { get; }

        public AlbumAdapter(SimpleAlbum album)
        {
            Name = album.Name;
            ReleaseDate = DateTime.Parse(album.ReleaseDate);
            _albumArt = new Uri(album.Images.First().Url);
        }

        public void Dispose()
        {
            _artImage?.Dispose();
        }
    }
}