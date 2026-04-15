using System;
using System.Threading;
using System.Windows.Forms;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Spotitoast.Configuration;
using Spotitoast.Context;
using Spotitoast.Logic.Dependencies;

namespace Spotitoast
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var services = new ServiceCollection();
            services.AddSpotitoastCore();

            services.AddSingleton(sp =>
                sp.GetRequiredService<ConfigurationManager>()
                  .LoadConfiguration<HotkeysConfiguration>().GetAwaiter().GetResult());

            services.AddSingleton<SpotitoastContext>();

            using var serviceProvider = services.BuildServiceProvider();

            HotKeys.Handler.HotKeyHandler.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(serviceProvider.GetRequiredService<SpotitoastContext>());

            HotKeys.Handler.HotKeyHandler.Stop();
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(5));
            serviceProvider.GetRequiredService<IJobScheduler>().StopAsync(cancellationSource.Token).GetAwaiter().GetResult();
        }
    }
}