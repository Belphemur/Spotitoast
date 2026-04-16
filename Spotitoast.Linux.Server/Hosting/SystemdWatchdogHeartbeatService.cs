using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Spotitoast.Linux.Server.Hosting
{
    /// <summary>
    /// Sends explicit WATCHDOG notifications while running under systemd.
    /// This avoids relying on framework heartbeat behavior differences across versions.
    /// </summary>
    public class SystemdWatchdogHeartbeatService : BackgroundService
    {
        private readonly ISystemdNotifier? _systemdNotifier;
        private readonly ILogger<SystemdWatchdogHeartbeatService> _logger;

        public SystemdWatchdogHeartbeatService(
            ILogger<SystemdWatchdogHeartbeatService> logger,
            ISystemdNotifier? systemdNotifier = null)
        {
            _logger = logger;
            _systemdNotifier = systemdNotifier;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_systemdNotifier is null || !SystemdHelpers.IsSystemdService())
            {
                return;
            }

            var interval = ResolveHeartbeatInterval();
            if (!interval.HasValue)
            {
                _logger.LogWarning("Systemd watchdog heartbeat disabled because WATCHDOG_USEC is missing or invalid.");
                return;
            }

            _logger.LogInformation("Systemd watchdog heartbeat enabled with interval {Interval}.", interval.Value);

            using var timer = new PeriodicTimer(interval.Value);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _systemdNotifier.Notify(new ServiceState("WATCHDOG=1"));
            }
        }

        private static TimeSpan? ResolveHeartbeatInterval()
        {
            var watchdogUsec = Environment.GetEnvironmentVariable("WATCHDOG_USEC");
            if (!long.TryParse(watchdogUsec, NumberStyles.None, CultureInfo.InvariantCulture, out var microseconds) || microseconds <= 0)
            {
                return null;
            }

            var heartbeatTicks = TimeSpan.FromMicroseconds(microseconds).Ticks / 2;
            if (heartbeatTicks <= 0)
            {
                heartbeatTicks = TimeSpan.FromSeconds(1).Ticks;
            }

            return TimeSpan.FromTicks(heartbeatTicks);
        }
    }
}
