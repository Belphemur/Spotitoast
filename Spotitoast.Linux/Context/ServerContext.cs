using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Notify.Linux.Client;
using SoundSwitch.InterProcess.Communication.Protocol;
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

        public async Task EventLoopStart(string pipeName)
        {
            _notificationHandler.RegisterNotifications();
         
            while (true)
            {
                await using var namedPipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
                // Wait for a client to connect
                await namedPipeServer.WaitForConnectionAsync();
                var stringProtocol = new StreamString(namedPipeServer);
                var cmd = stringProtocol.ReadString();
                await HandleCommand(cmd);
            }
        }

        private async Task HandleCommand(string cmd)
        {
            var action = _commandExecutor.ParseCommand(cmd);
            if (!action.HasValue)
            {
                await _notificationClient.NotifyAsync(new SpotitoastNotification
                {
                    Body = $"Command: {cmd}\nAvailable: {string.Join(", ", _commandExecutor.AvailableCommands)}",
                    Summary = "Spotitoast Unknown command"
                });
                return;
            }

            await ExecuteCommand(action.Value);
        }

        private async Task ExecuteCommand(ActionKey action)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}