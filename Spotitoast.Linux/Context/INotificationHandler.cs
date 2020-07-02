namespace Spotitoast.Linux.Context
{
    public interface INotificationHandler
    {
        /// <summary>
        /// Link different event (Like, track played etc ...) to a notification
        /// </summary>
        void RegisterNotifications();
    }
}