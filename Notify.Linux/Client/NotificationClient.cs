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
        public Task Notify(string text)
        {
            return _notificationsClient.NotifyAsync("Spotitoast", _lastId++, String.Empty, text, String.Empty, new string[0], new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()), 0);
        }
        
        /// <summary>
        /// Notify 
        /// </summary>
        public Task Notify(NotificationData notification)
        {
            var hints = new Dictionary<string,object>();
            if (notification.Image != null)
            {
                hints.Add("image-data", notification.Image.ToPixbuf().ToIconData());
            }
            return _notificationsClient.NotifyAsync( notification.ApplicationName, _lastId++, String.Empty, notification.Summary, notification.Body, new string[0], hints, notification.Expiration);
        }
    }
}