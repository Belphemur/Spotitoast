﻿using System.Collections.Generic;

namespace Spotitoast.Logic.Model.Song
{
    public interface ITrack
    {
        IReadOnlyCollection<string> Artists { get; }
        string Name { get; }
        IAlbum Album { get; }
    }
}