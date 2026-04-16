using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Spotitoast.Shared;
using Spotitoast.Shared.Ipc;

namespace Spotitoast.Linux.Client.Commands;

/// <summary>
/// Generic "send" command: <c>spotitoast send Like</c>.
/// </summary>
public sealed class SendCommand : AsyncCommand<SendCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The command to send (Like, Dislike, TogglePlayback, CurrentlyPlaying, Skip, Exit).")]
        [CommandArgument(0, "<command>")]
        public string Command { get; init; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        if (!Enum.TryParse<PlayerCommand>(settings.Command, ignoreCase: true, out var command))
        {
            AnsiConsole.MarkupLine($"[red]Unknown command:[/] {Markup.Escape(settings.Command)}");
            AnsiConsole.MarkupLine("[dim]Available commands:[/] {0}",
                string.Join(", ", Enum.GetNames<PlayerCommand>()));
            return 1;
        }

        return await IpcHelper.SendAsync(command, cancellation);
    }
}
