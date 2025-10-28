# RobotControlServer
RobotControlServer is a lightweight web server that exposes HTTP/WebSocket endpoints to bridge your robot software (running on a shop-floor PC) with external clients (UIs, services, test tools).
Typical flow:

Client (browser/app)  →  RobotControlServer  →  Vendor robot app/device on local PC  →  Robot

Key features

HTTP/REST endpoints for connect, status, jog, move, stop, etc.

Optional WebSocket/SignalR channel for live telemetry & events.

Pluggable TCP/IP driver to talk to the vendor app or controller.

Works on Windows; deployable as folder publish or service.

Minimal dependencies; easy to run behind Cloudflare/nginx/IIS.

Architecture
+--------------------+
|  Client / Frontend |
+---------+----------+
          |
          | HTTPS / WebSocket (LAN / Internet)
          v
+---------+------------------+
|      RobotControlServer    |
|  • REST API (ASP.NET Core) |
|  • SignalR (optional)      |
|  • TCP client to vendor    |
+---------+------------------+
          |
          | TCP/UDP/SDK (local)
          v
+---------+------------------+
| Vendor App / Controller PC |
+----------------------------+

Requirements

Windows 10/11

.NET 8.0 Runtime (or self-contained publish)

If your project is .NET Framework, publish the folder with all DLLs.

Quick start
# 1) Configure
copy appsettings.Sample.json appsettings.json
# edit host/port and robot connection

# 2) Run (dev)
dotnet run --project src/RobotControlServer

# 3) Or publish (release)
dotnet publish -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true -p:PublishTrimmed=false


By default the server listens on http://localhost:5249 (configure below).

Configuration

Create an appsettings.json (do not commit secrets; keep a appsettings.Sample.json in the repo):

{
  "Server": {
    "Host": "0.0.0.0",
    "Port": 5249
  },
  "Robot": {
    "Host": "127.0.0.1",
    "Port": 29999,
    "Protocol": "TCP"    // or "UDP", "SDK", etc.
  },
  "Auth": {
    "Enabled": true,
    "Token": "CHANGE-ME"  // use environment variable in production
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000" ]
  },
  "Logging": {
    "LogLevel": { "Default": "Information" }
  }
}
