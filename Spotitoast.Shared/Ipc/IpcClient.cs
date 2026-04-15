using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spotitoast.Shared.Ipc;

/// <summary>
/// Lightweight TCP client for sending a single command to the Spotitoast server
/// and reading back the response string. Used by the CLI.
/// </summary>
public sealed class IpcClient : IDisposable
{
    private readonly TcpClient _tcp = new();

    public async Task ConnectAsync(int port, CancellationToken ct = default)
    {
        await _tcp.ConnectAsync(IPAddress.Loopback, port, ct);
    }

    public bool Connected => _tcp.Connected;

    /// <summary>
    /// Send a command string and return the server's response.
    /// </summary>
    public async Task<string> SendCommandAsync(string command, CancellationToken ct = default)
    {
        var stream = _tcp.GetStream();

        var data = Encoding.ASCII.GetBytes(command);
        await stream.WriteAsync(data, ct);

        // Read response
        var buffer = new byte[IpcConstants.BufferSize];
        var bytesRead = await stream.ReadAsync(buffer, ct);
        return bytesRead > 0
            ? Encoding.ASCII.GetString(buffer, 0, bytesRead)
            : string.Empty;
    }

    public void Dispose()
    {
        _tcp.Dispose();
    }
}
