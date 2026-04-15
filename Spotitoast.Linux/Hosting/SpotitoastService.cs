using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.Hosting;
using Ninject;
using Spotitoast.Linux.Context;
using Spotitoast.Linux.Notification;

namespace Spotitoast.Linux.Hosting
{
    /// <summary>
    /// Background service that runs the Spotitoast TCP server event loop
    /// and registers notification handlers for the Linux desktop.
    /// </summary>
    public class SpotitoastService : BackgroundService
    {
        private readonly IKernel _kernel;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly int _port;

        public SpotitoastService(IKernel kernel, IHostApplicationLifetime lifetime, int port)
        {
            _kernel = kernel;
            _lifetime = lifetime;
            _port = port;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Wire notification subscriptions before the host signals READY=1
            // so systemd considers the service fully operational.
            _kernel.Get<INotificationHandler>().RegisterNotifications();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _kernel.Get<ServerContext>()
                         .EventLoopStartAsync(_port, stoppingToken);

            // The event loop exits when the cancellation token fires or when
            // a client sends the Exit command.  In either case, ask the host
            // to tear down gracefully.
            _lifetime.StopApplication();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            await _kernel.Get<IJobScheduler>().StopAsync(cancellationToken);
        }
    }
}
