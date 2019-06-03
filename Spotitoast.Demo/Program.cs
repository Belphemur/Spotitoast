using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ninject;
using Spotitoast.Banner.Client;
using Spotitoast.Banner.Model;
using Spotitoast.Configuration;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Dependencies;

namespace Spotitoast
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var client = Bootstrap.Kernel.Get<ISpotifyNotifier>();
            var mainForm = Bootstrap.Kernel.Get<Form1>();

            client.TrackPlayed.Subscribe(async track =>
            {
                var trackName = track.Name;
                var artists = track.ArtistsDisplay;
                mainForm.UpdateTrackLabel(trackName);
                var bannerData = new BannerData()
                {
                    Title = trackName,
                    Text = track.Album.Name,
                    SubText = artists,
                    Image= await track.Album.Art
                };

                BannerClient.ShowNotification(bannerData);
            });


            Application.Run(mainForm);
        }
    }
}