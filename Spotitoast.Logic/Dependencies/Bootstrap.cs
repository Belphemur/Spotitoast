using Ninject;
using Ninject.Modules;
using Spotitoast.Logic.Dependencies.Actions;
using Spotitoast.Logic.Dependencies.Configuration;

namespace Spotitoast.Logic.Dependencies
{
    public static class Bootstrap
    {
        private static readonly INinjectModule[] _modules = {
            new ConfigurationModule(),
            new ActionModule()
        };

        public static IKernel Kernel => new StandardKernel(_modules);
    }
}