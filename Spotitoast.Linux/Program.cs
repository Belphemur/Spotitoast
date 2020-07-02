
using System;
using System.Drawing;
using System.Threading.Tasks;
using Ninject;
using Notify.Linux.Client;

namespace Spotitoast.Linux
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logic.Dependencies.Bootstrap.Kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            var notifclient = Logic.Dependencies.Bootstrap.Kernel.Get<INotificationClient>();
            var img = Image.FromFile("/tmp/images.png");
            await notifclient.Notify(new NotificationData()
            {
                ApplicationName = "Spotitoast",
                Body = "World",
                Summary = "Hello",
                Image = img
            });
        }
    }
}