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
using Spotitoast.Banner.Display;
using Spotitoast.Banner.Model;

namespace Spotitoast.Banner.Client
{
    /// <summary>
    /// Class to manage the banners. This class is the entrypoint to show a notification banner.
    /// </summary>
    public class BannerClient
    {
        private static System.Threading.SynchronizationContext _syncContext;
        private static BannerForm _banner;

        /// <summary>
        /// Show a banner notification with the given data
        /// </summary>
        /// <param name="data"></param>
        public static void ShowNotification(BannerData data)
        {
            // Execute the banner in the context of the UI thread
            _syncContext.Post((d) =>
            {
                if (_banner == null)
                {
                    _banner = new BannerForm();
                    _banner.Disposed += (s, e) =>
                    {
                        _banner = null;
                        data.Dispose();
                    };
                }
                _banner.SetData(data);
            }, null);
        }

        /// <summary>
        /// Because notifications dispatched asynchronously, this method must be called in the context of the UI thread
        /// <remarks>This method requires that at least one System.Windows.Form.Control has been created or Application.Run() called</remarks>
        /// </summary>
        public static void Setup()
        {
            // Grab the synchronization context of the UI thread!
            _syncContext = System.Threading.SynchronizationContext.Current;
            if (!(_syncContext is System.Windows.Forms.WindowsFormsSynchronizationContext))
                throw new InvalidOperationException("BannerManager must be called in the context of the UI thread.");
        }
    }
}