using System.Threading.Tasks;

namespace Notify.Linux.Client
{
    public interface INotificationClient
    {
        /// <summary>
        /// Notify 
        /// </summary>
        Task Notify(string text);

        /// <summary>
        /// Notify 
        /// </summary>
        Task Notify(NotificationData notification);
    }
}