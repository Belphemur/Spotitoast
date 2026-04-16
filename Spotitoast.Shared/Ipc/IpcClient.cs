using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spotitoast.Shared.Ipc;

/// <summary>
/// Lightweight named-pipe client for sending a single command to the Spotitoast server
/// and reading back the response string. Used by the CLI.
/// </summary>
public sealed class IpcClient : IDisposable
{
    private readonly NamedPipeClientStream _pipe = new(".", IpcConstants.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        await _pipe.ConnectAsync(ct);
    }

    public bool Connected => _pipe.IsConnected;

    /// <summary>
    /// Send a command string and return the server's response.
    /// </summary>
    public async Task<string> SendCommandAsync(string command, CancellationToken ct = default)
    {
        var data = Encoding.ASCII.GetBytes(command);
        await _pipe.WriteAsync(data, ct);

        // Read response
        var buffer = new byte[IpcConstants.BufferSize];
        var bytesRead = await _pipe.ReadAsync(buffer, ct);
        return bytesRead > 0
            ? Encoding.ASCII.GetString(buffer, 0, bytesRead)
            : string.Empty;
    }

    public void Dispose()
    {
        _pipe.Dispose();
    }
}
