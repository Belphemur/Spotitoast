namespace Spotitoast.Linux.Notification
{
    public interface INotificationHandler
    {
        /// <summary>
        /// Link different event (Like, track played etc ...) to a notification
        /// </summary>
        void RegisterNotifications();
    }
}