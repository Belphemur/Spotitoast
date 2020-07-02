using System.Threading.Tasks;

namespace Notify.Linux.Client
{
    public interface INotificationClient
    {
        /// <summary>
        /// Notify 
        /// </summary>
        Task NotifyAsync(string text);

        /// <summary>
        /// Notify 
        /// </summary>
        Task NotifyAsync(NotificationData notification);
    }
}