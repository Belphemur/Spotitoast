using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Spotitoast.Logic.Business.Action;

namespace Spotitoast.Linux.Context
{
    public class ClientContext : IDisposable
    {
        private readonly IActionFactory _actionFactory;
        private readonly TcpClient _tcpClient;

        public ClientContext(IActionFactory actionFactory)
        {
            _actionFactory = actionFactory;
            _tcpClient = new TcpClient();
        }

        public Task ConnectAsync(int port)
        {
            return _tcpClient.ConnectAsync(IPAddress.Loopback, port);
        }

        /// <summary>
        /// Send command to server
        /// </summary>
        /// <param name="args"></param>
        public async Task SendCommand(string[] args)
        {
            if (args.Length == 0)
            {
                await Console.Out.WriteLineAsync($"Please use one of the following commands: {string.Join(", ", _actionFactory.AvailableKeys)}");
                Environment.Exit(1);
                return;
            }

            if (!_tcpClient.Connected)
            {
                await Console.Out.WriteLineAsync($"Please connect to the server first");
                Environment.Exit(1);
                return;
            }

            var stream = _tcpClient.GetStream();
            var data = System.Text.Encoding.ASCII.GetBytes(args[0]);
            await stream.WriteAsync(data, 0, data.Length);
            Environment.Exit(0);
        }

        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}