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
            var pipeName = $"Spotitoast-{Environment.UserName}";
            using var mutex = new Mutex(true, @$"Global\{pipeName}", out var createdNew);
            //When creating the mutex, we run a server
            if (createdNew)
            {
                await Console.Out.WriteLineAsync("Running as server.");
                await Logic.Dependencies.Bootstrap.Kernel.Get<ServerContext>().EventLoopStart(pipeName);
                await Logic.Dependencies.Bootstrap.Kernel.Get<IJobScheduler>().StopAsync();
                return;
            }

            await Logic.Dependencies.Bootstrap.Kernel.Get<ClientContext>().SendCommand(pipeName, args);
        }
    }
}