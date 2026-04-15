using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;
using Spotitoast.Linux.Bootstrap;
using Spotitoast.Linux.Context;
using Spotitoast.Linux.Hosting;
using Spotitoast.Linux.Notification;
using Spotitoast.Logic.Business.Action;
using Spotitoast.Logic.Dependencies;

namespace Spotitoast.Linux
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mutexName = $"Spotitoast-{Environment.UserName}";
            var mutex = new Mutex(true, @$"Global\{mutexName}", out var createdNew);
            var port = Port();

            try
            {
                // Subsequent instances act as lightweight TCP clients that
                // forward a single command to the already-running server.
                if (!createdNew)
                {
                    await SendClientCommand(args, port);
                    return;
                }

                await RunServer(args, port);
            }
            finally
            {
                if (createdNew)
                {
                    mutex.ReleaseMutex();
                }

                mutex.Dispose();
            }
        }

        private static async Task SendClientCommand(string[] args, int port)
        {
            var services = new ServiceCollection();
            services.AddSpotitoastCore();
            await using var sp = services.BuildServiceProvider();

            var factory = sp.GetRequiredService<IActionFactory>();
            using var clientContext = new ClientContext(factory);
            await clientContext.ConnectAsync(port);
            await clientContext.SendCommand(args);
        }

        private static async Task RunServer(string[] args, int port)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // When launched by systemd the extension sends READY=1,
            // STOPPING=1, STATUS= and WATCHDOG=1 notifications
            // automatically.  Outside systemd it is a harmless no-op.
            builder.Services.AddSystemd();

            // Core business-logic services (Spotify, actions, etc.)
            builder.Services.AddSpotitoastCore();

            // Linux-specific services (DBus notifications)
            builder.Services.AddSpotitoastLinux();

            // TCP server context
            builder.Services.AddSingleton<ServerContext>();

            // Hosted services
            builder.Services.AddHostedService(sp =>
                new SpotitoastService(
                    sp.GetRequiredService<IHostApplicationLifetime>(),
                    sp.GetRequiredService<INotificationHandler>(),
                    sp.GetRequiredService<ServerContext>(),
                    sp.GetRequiredService<IJobScheduler>(),
                    port));

            builder.Services.AddHostedService(sp =>
                new SystemdStatusReporter(
                    sp.GetRequiredService<Logic.Business.Player.ISpotifyNotifier>(),
                    sp.GetService<ISystemdNotifier>()));

            await Console.Out.WriteLineAsync($"Running as server on port {port}");

            var host = builder.Build();
            await host.RunAsync();
        }

        private static int Port()
        {
            var md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(Environment.UserName));
            var intValue = BitConverter.ToInt32(hashed, 0);
            var random = new Random(intValue);
            return random.Next(20000, 21000);
        }
    }
}