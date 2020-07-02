using System.IO;
using Notify.Linux.Client;

namespace Spotitoast.Linux.Notification
{
    public class SpotitoastNotification : NotificationData
    {
        public SpotitoastNotification()
        {
            ApplicationName = "Spotitoast";
            ApplicationIconPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location)}/Resources/spotitoast.ico";
        }
    }
}