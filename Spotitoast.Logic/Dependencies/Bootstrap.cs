using Ninject;

namespace Spotitoast.Logic.Dependencies
{
    public static class Bootstrap
    {
        public static IKernel Kernel = new StandardKernel();
    }
}