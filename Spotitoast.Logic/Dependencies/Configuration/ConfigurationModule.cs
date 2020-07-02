using System;
using System.IO;
using Ninject.Modules;
using Spotitoast.Configuration;
using Spotitoast.Spotify.Configuration;

namespace Spotitoast.Logic.Dependencies.Configuration
{
    public class ConfigurationModule : NinjectModule
    {
        public override void Load()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configManager = new ConfigurationManager(Path.Combine(folderPath, "Spotitoast"));

            Bind<ConfigurationManager>().ToConstant(configManager);
            Bind<SpotifyAuthConfiguration>().ToMethod(context => configManager.LoadConfiguration<SpotifyAuthConfiguration>().GetAwaiter().GetResult()).InSingletonScope();
            Bind<SpotifyWebClientConfiguration>().ToMethod(context => configManager.LoadConfiguration<SpotifyWebClientConfiguration>().GetAwaiter().GetResult()).InSingletonScope();
        }
    }
}