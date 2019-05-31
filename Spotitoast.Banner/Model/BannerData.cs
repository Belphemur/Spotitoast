/********************************************************************
* Copyright (C) 2015-2017 Antoine Aflalo
*
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version 2
* of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
********************************************************************/

using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Spotitoast.Banner.Model
{
    /// <summary>
    /// Contains configuration data for the banner form.
    /// </summary>
    public class BannerData : IDisposable
    {
        /// <summary>
        /// Gets/sets the title of the banner
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the text of the banner
        /// </summary>
        public string Text { get; set; } = "";

        /// <summary>
        /// Subtext to be shown
        /// </summary>
        public string SubText { get; set; } = "";

        /// <summary>
        /// Gets/sets the path for an image, this is optional.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// How long does the banner stays on screen
        /// </summary>
        public TimeSpan TimeOnScreen { get; set; } = TimeSpan.FromSeconds(5);

        public BannerData()
        {
        }

        public void Dispose()
        {
            Image?.Dispose();
        }
    }
}