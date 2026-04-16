using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
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
    await Console.Error.WriteLineAsync("Another Spotitoast server instance is already running for this user.");
    return 1;
}

var builder = Host.CreateApplicationBuilder(args);

// When launched by systemd the extension sends READY=1,
// STOPPING=1, STATUS= and WATCHDOG=1 notifications
// automatically.  Outside systemd it is a harmless no-op.
builder.Services.AddSystemd();

// Core business-logic services (Spotify, actions, etc.)
builder.Services.AddSpotitoastCore();

// Linux-specific services (DBus notifications)
builder.Services.AddSpotitoastLinux();

// Named-pipe server context
builder.Services.AddSingleton<ServerContext>();

// Hosted services
builder.Services.AddHostedService<SpotitoastService>();

builder.Services.AddHostedService(sp =>
    new SystemdStatusReporter(
        sp.GetRequiredService<ISpotifyNotifier>(),
        sp.GetService<ISystemdNotifier>()));

await Console.Out.WriteLineAsync($"Running as server on pipe '{IpcConstants.PipeName}'");

var host = builder.Build();
await host.RunAsync();

return 0;