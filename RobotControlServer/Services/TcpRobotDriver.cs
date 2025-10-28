using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RobotControlServer.Services;

public class TcpRobotDriver : IRobotDriver
{
    private readonly RobotOptions _opt;
    private readonly ILogger<TcpRobotDriver> _log;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private Encoding _enc = Encoding.ASCII;

    public TcpRobotDriver(IOptions<RobotOptions> opt, ILogger<TcpRobotDriver> log)
    {
        _opt = opt.Value;
        _log = log;

        _enc = (_opt.Encoding ?? "ASCII").ToUpperInvariant() switch
        {
            "UTF8" or "UTF-8" => Encoding.UTF8,
            "UNICODE" or "UTF16" or "UTF-16" => Encoding.Unicode,
            _ => Encoding.ASCII
        };
    }

    public bool IsConnected => _client?.Connected == true && _stream != null;

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        if (IsConnected) return;

        _client = new TcpClient();
        using var cts = new CancellationTokenSource(_opt.ConnectTimeoutMs); // sadece kendi timeout
        try
        {
            await _client.ConnectAsync(_opt.Host, _opt.Port, cts.Token);
            _stream = _client.GetStream();
            _log.LogInformation("TCP connected to {Host}:{Port}", _opt.Host, _opt.Port);
            Console.WriteLine($"[TcpRobotDriver] Connected {_opt.Host}:{_opt.Port}");
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Connect timeout to {_opt.Host}:{_opt.Port}");
        }
    }

    public async Task DisconnectAsync()
    {
        try { _stream?.Close(); } catch { }
        try { _client?.Close(); } catch { }
        _stream = null;
        _client = null;
        await Task.CompletedTask;
        _log.LogInformation("TCP disconnected");
    }

    public async Task<string> PingAsync(string message, CancellationToken ct = default)
    {
        await SendAsync(message, ct);
        return $"[SENT]: {message}";
    }

    public async Task SendAsync(string text, CancellationToken ct = default)
    {
        try
        {
            if (!IsConnected) await ConnectAsync(ct);
            if (_stream == null) throw new InvalidOperationException("TCP stream not available");
            string payload = _opt.AppendNewLine ? text + "\r\n" : text;
            //var payload = _opt.AppendNewLine ? text + "\n" : text;
            var data = _enc.GetBytes(payload);

            await _stream.WriteAsync(data, 0, data.Length, ct);
            await _stream.FlushAsync(ct);

            _log.LogInformation("TX → {Text}", text);
            Console.WriteLine($"[TcpRobotDriver] TX → {text}");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "SendAsync failed");
            Console.WriteLine($"[TcpRobotDriver] ERROR: {ex.Message}");
            throw;
        }
    }
}
