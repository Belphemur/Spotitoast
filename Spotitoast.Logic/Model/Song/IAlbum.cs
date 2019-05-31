using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Spotitoast.Logic.Model.Song
{
    public interface IAlbum : IDisposable
    {
        /// <summary>
        /// Album Art
        /// </summary>
        Task<Image> Art { get; }

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