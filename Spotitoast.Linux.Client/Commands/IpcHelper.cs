using System.Net.Sockets;
using Spectre.Console;
using Spotitoast.Shared;
using Spotitoast.Shared.Ipc;

namespace Spotitoast.Linux.Client.Commands;

/// <summary>
/// Shared helper that connects to the server, sends a command, and prints the result.
/// </summary>
internal static class IpcHelper
{
    /// <summary>
    /// Send a <see cref="PlayerCommand"/> to the running Spotitoast server and
    /// return the CLI exit code (0 = success).
    /// </summary>
    public static async Task<int> SendAsync(PlayerCommand command)
    {
        var port = IpcConstants.Port();

        using var client = new IpcClient();
        try
        {
            await client.ConnectAsync(port);
        }
        catch (SocketException)
        {
            AnsiConsole.MarkupLine("[red]Could not connect to the Spotitoast server.[/]");
            AnsiConsole.MarkupLine("[dim]Is the service running? Start it with:[/]  systemctl --user start spotitoast");
            return 1;
        }

        var response = await client.SendCommandAsync(command.ToString());

        if (string.IsNullOrEmpty(response))
        {
            AnsiConsole.MarkupLine("[yellow]No response from server.[/]");
            return 1;
        }

        // Response is the ActionResult enum name
        switch (response)
        {
            case "Success":
                AnsiConsole.MarkupLine($"[green]✓[/] {command}");
                return 0;
            case "NoTrackPlayed":
                AnsiConsole.MarkupLine("[yellow]No track is currently playing.[/]");
                return 1;
            case "AlreadyLiked":
                AnsiConsole.MarkupLine("[yellow]Track is already liked.[/]");
                return 0;
            case "NotLiked":
                AnsiConsole.MarkupLine("[yellow]Track is not liked.[/]");
                return 0;
            case "ExitApplication":
                AnsiConsole.MarkupLine("[green]Server is shutting down.[/]");
                return 0;
            case "Error":
                AnsiConsole.MarkupLine("[red]Server returned an error.[/]");
                return 1;
            default:
                AnsiConsole.MarkupLine($"[dim]Server response:[/] {response}");
                return 0;
        }
    }

    /// <summary>
    /// Check whether the server is reachable on the expected port.
    /// </summary>
    public static async Task<bool> IsServerRunningAsync()
    {
        var port = IpcConstants.Port();
        using var client = new IpcClient();
        try
        {
            await client.ConnectAsync(port);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}
