using System.IO;
using Notify.Linux.Client;

namespace Spotitoast.Linux.Server.Notification
{
    public class SpotitoastNotification : NotificationData
    {
        public SpotitoastNotification()
        {
            ApplicationName = "Spotitoast";
            var appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!;
            ApplicationIconPath = Path.Combine(appDirectory, "Spotitoast.ico");
            Hints.Add("desktop-entry", "Spotitoast");
        }
    }
}