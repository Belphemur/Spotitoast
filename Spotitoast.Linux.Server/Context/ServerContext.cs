using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notify.Linux.Client;
using Spotitoast.Linux.Server.Notification;
using Spotitoast.Logic.Business.Action.Implementation;
using Spotitoast.Logic.Business.Command;
using Spotitoast.Shared.Ipc;
using Spotitoast.Spotify.Model;

#nullable enable

namespace Spotitoast.Linux.Server.Context
{
    public class ServerContext
    {
        private readonly INotificationClient _notificationClient;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ILogger<ServerContext> _logger;
        private readonly object _pipeServerLock = new object();
        private NamedPipeServerStream? _pipeServer;

        public ServerContext(ICommandExecutor commandExecutor, INotificationClient notificationClient, ILogger<ServerContext> logger)
        {
            _commandExecutor = commandExecutor;
            _notificationClient = notificationClient;
            _logger = logger;
        }

        public async Task EventLoopStartAsync(CancellationToken token)
        {
            var bytes = new byte[IpcConstants.BufferSize];

            while (!token.IsCancellationRequested)
            {
                await using var pipeServer = new NamedPipeServerStream(
                    IpcConstants.PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                SetCurrentPipeServer(pipeServer);

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
                catch (IOException e)
                {
                    _logger.LogDebug(e, "Client disconnected unexpectedly.");
                }
                catch (ObjectDisposedException) when (token.IsCancellationRequested)
                {
                    return;
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    return;
                }
                finally
                {
                    ClearCurrentPipeServer(pipeServer);
                }
            }
        }

        public void RequestShutdown()
        {
            lock (_pipeServerLock)
            {
                _pipeServer?.Dispose();
                _pipeServer = null;
            }
        }

        private async Task<ActionResult> HandleCommand(string cmd)
        {
            _logger.LogInformation("Received client command {Command}", cmd);

            var action = _commandExecutor.ParseCommand(cmd);
            if (!action.HasValue)
            {
                var availableCommands = string.Join(", ", _commandExecutor.AvailableCommands);
                _logger.LogWarning("Unknown client command {Command}. Available commands: {AvailableCommands}", cmd, availableCommands);

                await _notificationClient.NotifyAsync(new SpotitoastNotification
                {
                    Body = $"Command: {cmd}\nAvailable: {availableCommands}",
                    Summary = "Spotitoast Unknown command"
                });
                return ActionResult.Error;
            }

            var result = await ExecuteCommand(action.Value);
            _logger.LogInformation("Command {Command} completed with {Result}", cmd, result);

            return result;
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
                    _logger.LogError("Couldn't execute action {Action}", action);
                    break;
                case ActionResult.ExitApplication:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private void SetCurrentPipeServer(NamedPipeServerStream pipeServer)
        {
            lock (_pipeServerLock)
            {
                _pipeServer = pipeServer;
            }
        }

        private void ClearCurrentPipeServer(NamedPipeServerStream pipeServer)
        {
            lock (_pipeServerLock)
            {
                if (ReferenceEquals(_pipeServer, pipeServer))
                {
                    _pipeServer = null;
                }
            }
        }
    }
}