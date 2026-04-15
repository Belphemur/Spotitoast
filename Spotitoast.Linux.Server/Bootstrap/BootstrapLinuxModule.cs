using Microsoft.Extensions.DependencyInjection;
using Notify.Linux.Client;
using Spotitoast.Linux.Server.Notification;
using Tmds.DBus;

namespace Spotitoast.Linux.Server.Bootstrap
{
    public static class BootstrapLinux
    {
        /// <summary>
        /// Register Linux-specific services (DBus notifications).
        /// </summary>
        public static IServiceCollection AddSpotitoastLinux(this IServiceCollection services)
        {
            services.AddSingleton<INotificationClient>(_ => new NotificationClient(Connection.Session));
            services.AddSingleton<INotificationHandler, NotificationHandler>();
            return services;
        }
    }
}