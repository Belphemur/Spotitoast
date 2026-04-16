using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging;
using Spotitoast.Linux.Server.Bootstrap;
using Spotitoast.Linux.Server.Context;
using Spotitoast.Linux.Server.Hosting;
using Spotitoast.Linux.Server.Notification;
using Spotitoast.Logic.Business.Player;
using Spotitoast.Logic.Dependencies;
using Spotitoast.Shared.Ipc;

using var mutex = new Mutex(true, IpcConstants.MutexName, out var createdNew);
if (!createdNew)
{
    using var loggerFactory = LoggerFactory.Create(logging => logging.AddSimpleConsole());
    var bootstrapLogger = loggerFactory.CreateLogger("Spotitoast.Linux.Server");
    bootstrapLogger.LogError("Another Spotitoast server instance is already running for this user.");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);

// Enable integration with systemd notifications. This wires host lifetime
// readiness/stopping semantics and registers ISystemdNotifier.
builder.Services.AddSystemd();

// Core business-logic services (Spotify, actions, etc.)
builder.Services.AddSpotitoastCore();

// Linux-specific services (DBus notifications)
builder.Services.AddSpotitoastLinux();

// Named-pipe server context
builder.Services.AddSingleton<ServerContext>();

// Hosted services
builder.Services.AddHostedService<SpotitoastService>();
builder.Services.AddHostedService<SystemdWatchdogHeartbeatService>();

builder.Services.AddHostedService(sp =>
    new SystemdStatusReporter(
        sp.GetRequiredService<ISpotifyNotifier>(),
        sp.GetService<ISystemdNotifier>()));

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Spotitoast.Linux.Server");
logger.LogInformation("Running as server on pipe {PipeName}", IpcConstants.PipeName);
await host.RunAsync();

return 0;