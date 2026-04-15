using System;
using System.Security.Cryptography;
using System.Text;

namespace Spotitoast.Shared.Ipc;

/// <summary>
/// Shared IPC constants and port computation used by both server and CLI.
/// The port is deterministically derived from the current username so
/// multiple users on the same host do not collide.
/// </summary>
public static class IpcConstants
{
    public const int BufferSize = 256;

    /// <summary>
    /// Compute the deterministic localhost port for the current user.
    /// </summary>
    public static int Port()
    {
        var hashed = MD5.HashData(Encoding.UTF8.GetBytes(Environment.UserName));
        var intValue = BitConverter.ToInt32(hashed, 0);
        var random = new Random(intValue);
        return random.Next(20000, 21000);
    }

    /// <summary>
    /// The named mutex that ensures only one server instance runs per user.
    /// </summary>
    public static string MutexName => $@"Global\Spotitoast-{Environment.UserName}";
}
