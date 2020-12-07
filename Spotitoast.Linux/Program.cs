using System;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
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
            var port = Port();
            if (createdNew)
            {
                await RunServer(port);
                return;
            }

            await SendClientCommand(args, port);
            Environment.Exit(0);
        }

        private static async Task SendClientCommand(string[] args, int port)
        {
            using var clientContext = Logic.Dependencies.Bootstrap.Kernel.Get<ClientContext>();
            await clientContext.ConnectAsync(port);
            await clientContext.SendCommand(args);
        }

        private static async Task RunServer(int port)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
            };
            AssemblyLoadContext.Default.Unloading += _ => cts.Cancel();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => { cts.Cancel(); };

            await Console.Out.WriteLineAsync($"Running as server on port {port}");
            await Logic.Dependencies.Bootstrap.Kernel.Get<ServerContext>().EventLoopStartAsync(port, cts.Token);
            await Logic.Dependencies.Bootstrap.Kernel.Get<IJobScheduler>().StopAsync(cts.Token);
            Environment.Exit(0);
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