using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spotitoast.Banner.Client;
using Spotitoast.Banner.Model;
using Spotitoast.Configuration;
using Spotitoast.Spotify.Client;

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

            var configManager = new ConfigurationManager(Path.Combine(folderPath, "Spotitoast"));

            var client = new SpotifyClient(configManager);

            var mainForm = new Form1(client);


            client.CurrentlyPlayedTrackChanged += (sender, @event) =>
            {
                var trackName = @event.Track.Name;
                var artists = string.Join(", ", @event.Track.Artists.Select(artist => artist.Name));
                var imageUrl = @event.Track.Album.Images.First().Url;
                mainForm.UpdateTrackLabel(trackName);
                var bannerData = new BannerData(imageUrl, new Size(100, 100))
                {
                    Title = trackName,
                    Text = @event.Track.Album.Name,
                    SubText = artists
                };

                BannerClient.ShowNotification(bannerData);
            };


            Application.Run(mainForm);
        }
    }
}