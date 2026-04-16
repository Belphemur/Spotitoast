using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Notify.Linux.Client;
using Spotitoast.Linux.Server.Notification;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Shared.Ipc;
using Spotitoast.Spotify.Model;

namespace Spotitoast.Linux.Server.Context
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

        public async Task EventLoopStartAsync(CancellationToken token)
        {
            _notificationHandler.RegisterNotifications();
            var bytes = new byte[IpcConstants.BufferSize];

            while (!token.IsCancellationRequested)
            {
                await using var pipeServer = new NamedPipeServerStream(
                    IpcConstants.PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                try
                {
                    await pipeServer.WaitForConnectionAsync(token);

                    int bytesRead;
                    while ((bytesRead = await pipeServer.ReadAsync(bytes.AsMemory(0, bytes.Length), token)) != 0)
                    {
                        var cmd = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

                        var commandResult = await HandleCommand(cmd);
                        var msg = System.Text.Encoding.ASCII.GetBytes(commandResult.ToString());

                        await pipeServer.WriteAsync(msg.AsMemory(0, msg.Length), token);
                        await pipeServer.FlushAsync(token);

                        if (commandResult == ActionResult.ExitApplication)
                        {
                            return;
                        }
                    }
                }
                catch (IOException)
                {
                    // Client disconnected unexpectedly — loop back and wait for next.
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    return;
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