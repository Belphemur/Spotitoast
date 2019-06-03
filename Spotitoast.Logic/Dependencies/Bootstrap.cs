using Ninject;
using Ninject.Modules;
using Spotitoast.Logic.Dependencies.Actions;
using Spotitoast.Logic.Dependencies.Configuration;
using Spotitoast.Logic.Dependencies.Spotify;

namespace Spotitoast.Logic.Dependencies
{
    public static class Bootstrap
    {
        private static readonly INinjectModule[] _modules =
        {
            new ConfigurationModule(),
            new ActionModule(),
            new SpotifyModule(),
        };

        public static IKernel Kernel = new StandardKernel(_modules);
    }
}