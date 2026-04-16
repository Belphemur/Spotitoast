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
    public class SpotitoastService : BackgroundService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly INotificationHandler _notificationHandler;
        private readonly ServerContext _serverContext;
        private readonly IJobScheduler _jobScheduler;

        public SpotitoastService(
            IHostApplicationLifetime lifetime,
            INotificationHandler notificationHandler,
            ServerContext serverContext,
            IJobScheduler jobScheduler)
        {
            _lifetime = lifetime;
            _notificationHandler = notificationHandler;
            _serverContext = serverContext;
            _jobScheduler = jobScheduler;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Wire notification subscriptions before the host signals READY=1
            // so systemd considers the service fully operational.
            _notificationHandler.RegisterNotifications();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serverContext.EventLoopStartAsync(stoppingToken);

            // The event loop exits when the cancellation token fires or when
            // a client sends the Exit command.  In either case, ask the host
            // to tear down gracefully.
            _lifetime.StopApplication();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            await _jobScheduler.StopAsync(cancellationToken);
        }
    }
}
