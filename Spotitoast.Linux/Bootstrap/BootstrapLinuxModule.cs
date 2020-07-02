using Ninject.Modules;
using Notify.Linux.Client;
using Spotitoast.Linux.Command;
using Spotitoast.Linux.Context;
using Tmds.DBus;

namespace Spotitoast.Linux.Bootstrap
{
    public class BootstrapLinuxModule : NinjectModule
    {
        public override void Load()
        {
            Bind<INotificationClient>().ToMethod(context => new NotificationClient(Connection.Session)).InSingletonScope();
            Bind<INotificationHandler>().To<NotificationHandler>().InSingletonScope();
            Bind<ICommandExecutor>().To<CommandExecutor>().InSingletonScope();
        }
    }
}