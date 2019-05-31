using Ninject.Modules;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Spotify.Client;

namespace Spotitoast.Logic.Dependencies.Spotify
{
    public class SpotifyModule : NinjectModule
    {
        
        public override void Load()
        {
            Bind<SpotifyClient>().To<SpotifyClient>().InSingletonScope();
            Bind<ISpotifyPlayer>().To<SpotifyPlayer>().InSingletonScope();
        }
    }
}