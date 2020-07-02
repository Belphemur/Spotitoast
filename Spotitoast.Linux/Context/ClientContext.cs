using System;
using System.Threading.Tasks;
using SoundSwitch.InterProcess.Communication;
using Spotitoast.Logic.Business.Action;

namespace Spotitoast.Linux.Context
{
    public class ClientContext
    {
        private readonly IActionFactory _actionFactory;

        public ClientContext(IActionFactory actionFactory)
        {
            _actionFactory = actionFactory;
        }

        /// <summary>
        /// Send command to server
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="args"></param>
        public async Task SendCommand(string pipeName, string[] args)
        {
            if (args.Length == 0)
            {
                await Console.Out.WriteLineAsync($"Please use one of the following commands: {string.Join(", ", _actionFactory.AvailableKeys)}");
                Environment.Exit(1);
                return;
            }
            using var client = new NamedPipeClient(pipeName);
            client.SendMsg(args[0]);
            Environment.Exit(0);
        }
    }
}