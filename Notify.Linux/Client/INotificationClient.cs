using System.Threading.Tasks;

namespace Notify.Linux.Client
{
    public interface INotificationClient
    {
        /// <summary>
        /// Notify
        /// <returns>ID of the sent notification</returns>
        /// </summary>
        Task<uint> NotifyAsync(NotificationData notification);
    }
}