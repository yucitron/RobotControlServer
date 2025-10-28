using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RobotControlServer.Services;

namespace RobotControlServer.Hubs
{
    public class RobotHub : Hub
    {
        private readonly IRobotDriver _robot;
        private readonly ILogger<RobotHub> _logger;

        public RobotHub(IRobotDriver robot, ILogger<RobotHub> logger)
        {
            _robot = robot;
            _logger = logger;
        }

        public async Task ConnectRobot()
        {
            if (!_robot.IsConnected)
                await _robot.ConnectAsync(CancellationToken.None); // <— ÖNEMLİ
            await Clients.Caller.SendAsync("serverInfo", "Robot bağlantısı hazır.");

            _logger.LogInformation("Client {Conn} connected to robot.", Context.ConnectionId);
            Console.WriteLine($"[RobotHub] ConnectRobot from {Context.ConnectionId}");

            
        }

        public async Task<string> PingRobot(string msg)
        {
            var reply = await _robot.PingAsync(msg, Context.ConnectionAborted);
            _logger.LogInformation("Ping from {Conn}: {Msg}", Context.ConnectionId, msg);
            Console.WriteLine($"[RobotHub] PingRobot: {msg}");
            await Clients.Caller.SendAsync("robotReply", reply);
            return reply;
        }

        // A–E tuşları → TCP ile gönder
        public async Task ButtonPressed(string key)
        {
            // Örn. sadece harfi gönderiyoruz ("A", "B", ...)
            
            await _robot.SendAsync(key, CancellationToken.None);   // <— ÖNEMLİ
            await Clients.Caller.SendAsync("serverInfo", $"Server: '{key}' gönderildi");
            _logger.LogInformation("Button {Key} sent via TCP by {Conn}", key, Context.ConnectionId);
            Console.WriteLine($"[RobotHub] Button {key} → TCP SENT");

            await Clients.Caller.SendAsync("serverInfo", $"Server: '{key}' gönderildi");
        }
    }
}
