using System;
using System.Threading.Tasks;
using IronSoftware.Drawing;

namespace Spotitoast.Logic.Model.Song
{
    public interface IAlbum : IDisposable
    {
        /// <summary>
        /// Album Art
        /// </summary>
        Task<AnyBitmap> Art { get; }

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