using Ninject.Modules;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Business.Action.Implementation;

namespace Spotitoast.Logic.Dependencies.Actions
{
    public class ActionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IActionFactory>().To<ActionFactory>().InSingletonScope();
            Bind<IAction>().To<Like>();
            Bind<IAction>().To<Dislike>();
            Bind<IAction>().To<TogglePlayback>();
            Bind<IAction>().To<CurrentlyPlaying>();
            Bind<IAction>().To<Exit>();
            Bind<IAction>().To<SkipTrack>();
        }
    }
}