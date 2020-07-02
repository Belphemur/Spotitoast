using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Notify.Linux.DBus;
using Notify.Linux.Extensions;
using Tmds.DBus;

namespace Notify.Linux.Client
{
    public class NotificationClient : INotificationClient
    {
        private readonly INotifications _notificationsClient;
        private uint _lastId = 0;

        public NotificationClient(Connection connection)
        {
            _notificationsClient = connection.CreateProxy<INotifications>("org.freedesktop.Notifications", "/org/freedesktop/Notifications");
        }

        /// <summary>
        /// Notify 
        /// </summary>
        public Task NotifyAsync(string text)
        {
            return _notificationsClient.NotifyAsync("Spotitoast", _lastId++, String.Empty, text, String.Empty, new string[0], new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()), 0);
        }
        
        /// <summary>
        /// Notify 
        /// </summary>
        public Task NotifyAsync(NotificationData notification)
        {
            var hints = new Dictionary<string,object>();
            if (notification.Image != null)
            {
                hints.Add("image-data", notification.Image.ToPixbuf().ToIconData());
            }

            var notifId = notification.NotificationId;
            if (notification.NotificationId == 0)
            {
                notifId = _lastId++;
            }
            return _notificationsClient.NotifyAsync( notification.ApplicationName, notifId, notification.ApplicationIconPath, notification.Summary, notification.Body, new string[0], hints, notification.Expiration);
        }
    }
}