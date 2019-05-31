using System;
using System.Drawing;

namespace Spotitoast.Logic.Model.Song
{
    public interface IAlbum
    {
        /// <summary>
        /// Album Art
        /// </summary>
        Image Art { get; }

        /// <summary>
        /// Name of the album
        /// </summary>
        string Name { get; }

        /// <summary>
        /// When was the album released
        /// </summary>
        DateTime ReleaseDate { get; }
    }
}