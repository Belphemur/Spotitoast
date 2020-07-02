using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using SoundSwitch.InterProcess.Communication;
using SoundSwitch.InterProcess.Communication.Protocol;
using Spotitoast.Linux.Context;

namespace Spotitoast.Linux
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logic.Dependencies.Bootstrap.Kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            using var mutex = new Mutex(true, @"Global\Spotitoast", out var createdNew);

            var pipeName = "Spotitoast";
            if (createdNew)
            {
                var notificationThread = Logic.Dependencies.Bootstrap.Kernel.Get<INotificationHandler>();
                notificationThread.RegisterNotifications();

                while (true)
                {
                    await using var namedPipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
                    // Wait for a client to connect
                    await namedPipeServer.WaitForConnectionAsync();
                    var stringProtocol = new StreamString(namedPipeServer);
                    var cmd = stringProtocol.ReadString();
                    Console.WriteLine(cmd);
                }
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