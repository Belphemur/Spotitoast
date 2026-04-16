using Spectre.Console.Cli;
using Spotitoast.Shared;

namespace Spotitoast.Linux.Client.Commands;

public sealed class SkipCommand : AsyncCommand
{
    protected override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellation)
        => IpcHelper.SendAsync(PlayerCommand.Skip, cancellation);
}
