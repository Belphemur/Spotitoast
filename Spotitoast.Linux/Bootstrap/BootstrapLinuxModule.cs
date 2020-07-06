using Ninject.Modules;
using Notify.Linux.Client;
using Spotitoast.Linux.Notification;
using Tmds.DBus;

namespace Spotitoast.Linux.Bootstrap
{
    public class BootstrapLinuxModule : NinjectModule
    {
        public override void Load()
        {
            Bind<INotificationClient>().ToMethod(context => new NotificationClient(Connection.Session)).InSingletonScope();
            Bind<INotificationHandler>().To<NotificationHandler>().InSingletonScope();
        }
    }
}