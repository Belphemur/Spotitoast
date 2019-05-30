using Ninject.Modules;
using Spotitoast.Logic.Model.Action;
using Spotitoast.Logic.Model.Action.Implementation;

namespace Spotitoast.Logic.Dependencies.Actions
{
    public class ActionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IActionFactory>().To<ActionFactory>().InSingletonScope();
            Bind<IAction>().To<LikeAction>();
            Bind<IAction>().To<DislikeAction>();
            Bind<IAction>().To<ResumeAction>();
            Bind<IAction>().To<PauseAction>();
        }
    }
}