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
    public class SystemdWatchdogHeartbeatService(
        ILogger<SystemdWatchdogHeartbeatService> logger,
        ISystemdNotifier? systemdNotifier = null)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var notifySocket = Environment.GetEnvironmentVariable("NOTIFY_SOCKET");
            var watchdogPid = Environment.GetEnvironmentVariable("WATCHDOG_PID");

            if (systemdNotifier is null)
            {
                logger.LogWarning("Systemd watchdog heartbeat disabled because ISystemdNotifier is not registered.");
                return;
            }

            if (string.IsNullOrWhiteSpace(notifySocket))
            {
                logger.LogWarning("Systemd watchdog heartbeat disabled because NOTIFY_SOCKET is missing.");
                return;
            }

            if (TryParseWatchdogPid(watchdogPid, out var expectedPid) && expectedPid != Environment.ProcessId)
            {
                logger.LogWarning(
                    "Systemd watchdog heartbeat disabled because WATCHDOG_PID={WatchdogPid} does not match current pid {CurrentPid}.",
                    expectedPid,
                    Environment.ProcessId);
                return;
            }

            var interval = ResolveHeartbeatInterval();
            if (!interval.HasValue)
            {
                logger.LogWarning("Systemd watchdog heartbeat disabled because WATCHDOG_USEC is missing or invalid.");
                return;
            }

            logger.LogInformation(
                "Systemd watchdog heartbeat enabled with interval {Interval} (pid {Pid}, notify socket {NotifySocket}).",
                interval.Value,
                Environment.ProcessId,
                notifySocket);
            systemdNotifier.Notify(new ServiceState("READY=1"));
            logger.LogInformation("Systemd has been notified of readiness");

            systemdNotifier.Notify(new ServiceState("WATCHDOG=1"));

            using var timer = new PeriodicTimer(interval.Value);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    systemdNotifier.Notify(new ServiceState("WATCHDOG=1"));
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to send systemd watchdog heartbeat notification.");
                }
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

        private static bool TryParseWatchdogPid(string? watchdogPid, out int pid)
        {
            if (int.TryParse(watchdogPid, NumberStyles.None, CultureInfo.InvariantCulture, out pid) && pid > 0)
            {
                return true;
            }

            pid = 0;
            return false;
        }
    }
}
