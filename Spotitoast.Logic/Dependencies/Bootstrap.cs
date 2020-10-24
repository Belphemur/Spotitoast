using Ninject;

namespace Spotitoast.Logic.Dependencies
{
    public static class Bootstrap
    {
        public static readonly IKernel Kernel = new StandardKernel();
    }
}