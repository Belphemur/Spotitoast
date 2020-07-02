using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using SoundSwitch.InterProcess.Communication;
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

            if (createdNew)
            {
                await Logic.Dependencies.Bootstrap.Kernel.Get<ServerContext>().EventLoopStart(pipeName);
            }

            if (args.Length > 0)
            {
                using var client = new NamedPipeClient(pipeName);
                client.SendMsg(args[0]);
                Environment.Exit(0);
            }

            await Console.Error.WriteLineAsync("Server loaded and no command given.");
            Environment.Exit(1);
        }
    }
}