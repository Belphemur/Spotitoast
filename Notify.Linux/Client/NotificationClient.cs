using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notify.Linux.DBus;
using Notify.Linux.Extensions;
using Tmds.DBus;

namespace Notify.Linux.Client
{
    public class NotificationClient : INotificationClient
    {
        private readonly INotifications _notificationsClient;
        private readonly Dictionary<uint, Dictionary<string, NotificationData.Action>> _notificationActions = new Dictionary<uint, Dictionary<string, NotificationData.Action>>();

        public NotificationClient(Connection connection)
        {
            _notificationsClient = connection.CreateProxy<INotifications>("org.freedesktop.Notifications", "/org/freedesktop/Notifications");
            _notificationsClient.WatchNotificationClosedAsync(NotificationClosed).GetAwaiter().GetResult();
            _notificationsClient.WatchActionInvokedAsync(NotificationActionExecuted).GetAwaiter().GetResult();
        }

        private void NotificationActionExecuted((uint id, string actionKey) obj)
        {
            if (!_notificationActions.ContainsKey(obj.id))
            {
                return;
            }

            if (!_notificationActions[obj.id].TryGetValue(obj.actionKey, out var action))
            {
                return;
            }

            Task.Factory.StartNew(async o => await action.OnActionCalled.Invoke(), null);
        }

        private void NotificationClosed((uint id, uint reason) obj)
        {
            _notificationActions.Remove(obj.id);
        }

        /// <summary>
        /// Notify
        /// <returns>ID of the sent notification</returns>
        /// </summary>
        public async Task<uint> NotifyAsync(NotificationData notification)
        {
            var hints = notification.Hints;
            if (notification.Image != null && !hints.ContainsKey("image-data"))
            {
                hints.Add("image-data", notification.Image.ToPixbuf().ToIconData());
            }

            var actions = notification.Actions != null && notification.Actions.Length > 0
                ? notification.Actions.Select(action => new[] {action.Key, action.Label}).SelectMany(strings => strings).ToArray()
                : new string[0];

            var expiration = (int) (notification.Expiration?.TotalMilliseconds ?? -1);
            var notifId = await _notificationsClient.NotifyAsync(notification.ApplicationName, notification.NotificationId, notification.ApplicationIconPath, notification.Summary, notification.Body, actions, hints, expiration);

            if (notification.Actions == null || notification.Actions?.Length == 0)
            {
                return notifId;
            }

            _notificationActions.Add(notifId, notification.Actions!.ToDictionary(action => action.Key));

            return notifId;
        }
    }
}