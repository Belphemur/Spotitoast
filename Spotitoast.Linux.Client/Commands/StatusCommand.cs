using Spectre.Console;
using Spectre.Console.Cli;

namespace Spotitoast.Linux.Client.Commands;

/// <summary>
/// Check if the Spotitoast server is running.
/// </summary>
public sealed class StatusCommand : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellation)
    {
        var running = await IpcHelper.IsServerRunningAsync();

        if (running)
        {
            AnsiConsole.MarkupLine("[green]Spotitoast server is running.[/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[red]Spotitoast server is not running.[/]");
        AnsiConsole.MarkupLine("[dim]Start it with:[/]  systemctl --user start spotitoast");
        return 1;
    }
}
