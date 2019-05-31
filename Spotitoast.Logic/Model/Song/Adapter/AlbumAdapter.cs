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
        public Image Art { get; }
        public string Name { get; }
        public DateTime ReleaseDate { get; }

        public AlbumAdapter(SimpleAlbum album)
        {
            Name = album.Name;
            ReleaseDate = DateTime.Parse(album.ReleaseDate);
            Art = new Uri(album.Images.First().Url).AsImage();
        }
    }
}