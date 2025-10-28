namespace RobotControlServer.Services;

public interface IRobotDriver
{
    Task ConnectAsync(CancellationToken ct = default);
    Task DisconnectAsync();
    bool IsConnected { get; }
    // Basit bir “ping” iletisi: Web'den gelir, servis geri cevap döner
    Task<string> PingAsync(string message, CancellationToken ct = default);

    Task SendAsync(string text, CancellationToken ct = default);
}