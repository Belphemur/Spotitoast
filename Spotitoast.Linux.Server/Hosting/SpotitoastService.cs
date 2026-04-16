using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.Hosting;
using Spotitoast.Linux.Server.Context;
using Spotitoast.Linux.Server.Notification;

namespace Spotitoast.Linux.Server.Hosting
{
    /// <summary>
    /// Background service that runs the Spotitoast named-pipe server event loop
    /// and registers notification handlers for the Linux desktop.
    /// </summary>
    public class SpotitoastService(
        IHostApplicationLifetime lifetime,
        INotificationHandler notificationHandler,
        ServerContext serverContext,
        IJobScheduler jobScheduler)
        : BackgroundService
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Wire notification subscriptions before the host signals READY=1
            // so systemd considers the service fully operational.
            notificationHandler.RegisterNotifications();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Factory
                .StartNew(
                    () => serverContext.EventLoopStartAsync(stoppingToken),
                    stoppingToken,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
                .Unwrap();

            // The event loop exits when the cancellation token fires or when
            // a client sends the Exit command.  In either case, ask the host
            // to tear down gracefully.
            lifetime.StopApplication();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            serverContext.RequestShutdown();
            await base.StopAsync(cancellationToken);
            await jobScheduler.StopAsync(cancellationToken);
        }
    }
}
