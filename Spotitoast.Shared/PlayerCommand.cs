namespace Spotitoast.Shared;

/// <summary>
/// Commands that the CLI can send to the server.
/// These map 1:1 to <c>ActionFactory.PlayerAction</c> values.
/// The string representation is used as the wire protocol.
/// </summary>
public enum PlayerCommand
{
    Like,
    Dislike,
    TogglePlayback,
    CurrentlyPlaying,
    Exit,
    Skip
}
