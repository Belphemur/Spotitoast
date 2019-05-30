using Ninject.Modules;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Logic.Dependencies.Spotify
{
    public class SpotifyModule : NinjectModule
    {
        
        public override void Load()
        {
            Bind<SpotifyClient>().To<SpotifyClient>().InSingletonScope();
        }
    }
}