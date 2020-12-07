using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Notify.Linux.Client;
using Spotitoast.Linux.Notification;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Linux.Context
{
    public class ServerContext
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly INotificationClient _notificationClient;
        private readonly ICommandExecutor _commandExecutor;

        public ServerContext(ICommandExecutor commandExecutor, INotificationHandler notificationHandler, INotificationClient notificationClient)
        {
            _commandExecutor = commandExecutor;
            _notificationHandler = notificationHandler;
            _notificationClient = notificationClient;
        }

        public async Task EventLoopStartAsync(int port, CancellationToken token)
        {
            _notificationHandler.RegisterNotifications();
            var bytes = new Byte[256];
            String cmd;
            var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();
            while (!token.IsCancellationRequested)
            {
                using var client = await tcpListener.AcceptTcpClientAsync();
                var stream = client.GetStream();


                int i;

                // Loop to receive all the data sent by the client.
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length, token)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    cmd = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    var commandResult = await HandleCommand(cmd);
                    var msg = System.Text.Encoding.ASCII.GetBytes(commandResult.ToString());

                    // Send back a response.
                    await stream.WriteAsync(msg, 0, msg.Length, token);
                    if (commandResult == ActionResult.ExitApplication)
                    {
                        return;
                    }
                }
            }
        }

        private async Task<ActionResult> HandleCommand(string cmd)
        {
            var action = _commandExecutor.ParseCommand(cmd);
            if (!action.HasValue)
            {
                await _notificationClient.NotifyAsync(new SpotitoastNotification
                {
                    Body = $"Command: {cmd}\nAvailable: {string.Join(", ", _commandExecutor.AvailableCommands)}",
                    Summary = "Spotitoast Unknown command"
                });
                return ActionResult.Error;
            }

            return await ExecuteCommand(action.Value);
        }

        private async Task<ActionResult> ExecuteCommand(ActionKey action)
        {
            var result = await _commandExecutor.Execute(action);
            switch (result)
            {
                case ActionResult.Success:
                    break;
                case ActionResult.NoTrackPlayed:
                    await _notificationClient.NotifyAsync(new SpotitoastNotification
                    {
                        Body = $"No track playing",
                        Summary = "Spotitoast"
                    });
                    break;
                case ActionResult.AlreadyLiked:
                    await _notificationClient.NotifyAsync(new SpotitoastNotification
                    {
                        Body = $"Track already liked",
                        Summary = "Spotitoast"
                    });
                    break;
                case ActionResult.NotLiked:
                    break;
                case ActionResult.Error:
                    await Console.Out.WriteLineAsync($"Couldn't execute action {action}");
                    break;
                case ActionResult.ExitApplication:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}