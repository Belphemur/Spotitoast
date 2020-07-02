using System.Threading.Tasks;

namespace Notify.Linux.Client
{
    public interface INotificationClient
    {

        /// <summary>
        /// Notify 
        /// </summary>
        Task<uint> NotifyAsync(NotificationData notification);
    }
}