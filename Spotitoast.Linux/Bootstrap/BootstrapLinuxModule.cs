using Microsoft.Extensions.DependencyInjection;
using Notify.Linux.Client;
using Spotitoast.Linux.Notification;
using Tmds.DBus;

namespace Spotitoast.Linux.Bootstrap
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