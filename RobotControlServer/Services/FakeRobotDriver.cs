using Microsoft.Extensions.Logging;

namespace RobotControlServer.Services;

public class FakeRobotDriver : IRobotDriver
{
    private readonly ILogger<FakeRobotDriver> _log;
    public bool IsConnected { get; private set; }

    public FakeRobotDriver(ILogger<FakeRobotDriver> log) => _log = log;

    public Task ConnectAsync(CancellationToken ct = default)
    {
        IsConnected = true;
        _log.LogInformation("Fake robot connected.");
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        IsConnected = false;
        _log.LogInformation("Fake robot disconnected.");
        return Task.CompletedTask;
    }

    public Task<string> PingAsync(string message, CancellationToken ct = default)
    {
        var reply = $"[ROBOT]: {message.ToUpperInvariant()}";
        _log.LogInformation("Fake Ping -> {Reply}", reply);
        return Task.FromResult(reply);
    }

    public Task SendAsync(string text, CancellationToken ct = default)
    {
        _log.LogInformation("Fake SEND → {Text}", text);
        return Task.CompletedTask;
    }
}
