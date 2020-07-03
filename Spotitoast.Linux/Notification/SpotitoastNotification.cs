using System.IO;
using Notify.Linux.Client;

namespace Spotitoast.Linux.Notification
{
    public class SpotitoastNotification : NotificationData
    {
        public SpotitoastNotification()
        {
            ApplicationName = "Spotitoast";
            var resourceDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location)!, "Resources");
            ApplicationIconPath = Path.Combine(resourceDirectory, "spotitoast.ico");
            Hints.Add("desktop-entry", "spotitoast");
        }
    }
}