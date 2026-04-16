using System;

namespace Spotitoast.Shared.Ipc;

/// <summary>
/// Shared IPC constants used by both server and CLI.
/// Communication uses a named pipe unique to the current user.
/// </summary>
public static class IpcConstants
{
    public const int BufferSize = 256;

    /// <summary>
    /// The named pipe name, unique per user, used for server/client IPC.
    /// </summary>
    public static string PipeName => $"spotitoast-{Environment.UserName}";

    /// <summary>
    /// The named mutex that ensures only one server instance runs per user.
    /// </summary>
    public static string MutexName => $@"Global\Spotitoast-{Environment.UserName}";
}
