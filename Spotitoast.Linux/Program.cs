using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Scheduler;
using Ninject;
using Spotitoast.Linux.Context;

namespace Spotitoast.Linux
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logic.Dependencies.Bootstrap.Kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            var mutexName = $"Spotitoast-{Environment.UserName}";
            using var mutex = new Mutex(true, @$"Global\{mutexName}", out var createdNew);
            //When creating the mutex, we run a server
            var port = 25255;
            if (createdNew)
            {
                await RunServer(port);
                return;
            }

            await SendClientCommand(args, port);
        }

        private static async Task SendClientCommand(string[] args, int port)
        {
            using var clientContext = Logic.Dependencies.Bootstrap.Kernel.Get<ClientContext>();
            await clientContext.ConnectAsync(port);
            await clientContext.SendCommand(args);
        }

        private static async Task RunServer(int port)
        {
            Console.CancelKeyPress += async (_, _) =>
            {
                await Logic.Dependencies.Bootstrap.Kernel.Get<IJobScheduler>().StopAsync();
                Environment.Exit(0);
            };
            await Console.Out.WriteLineAsync("Running as server.");
            await Logic.Dependencies.Bootstrap.Kernel.Get<ServerContext>().EventLoopStart(port);
            await Logic.Dependencies.Bootstrap.Kernel.Get<IJobScheduler>().StopAsync();
        }
    }
}