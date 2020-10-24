using System;
using System.Threading;
using System.Windows.Forms;
using Job.Scheduler.Scheduler;
using Ninject;
using Spotitoast.Configuration;
using Spotitoast.Context;
using Spotitoast.HotKeys.Handler;
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
            var _ = new[] {typeof(IJobScheduler)};
            Bootstrap.Kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            Bootstrap.Kernel
                     .Bind<HotkeysConfiguration>()
                     .ToMethod(context =>
                         (context.Kernel.Get<ConfigurationManager>()).LoadConfiguration<HotkeysConfiguration>().GetAwaiter().GetResult())
                     .InSingletonScope();


            HotKeyHandler.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Bootstrap.Kernel.Get<SpotitoastContext>());

            HotKeyHandler.Stop();
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.CancelAfter(TimeSpan.FromSeconds(5));
            Bootstrap.Kernel.Get<IJobScheduler>().StopAsync(cancellationSource.Token).GetAwaiter().GetResult();
        }
    }
}