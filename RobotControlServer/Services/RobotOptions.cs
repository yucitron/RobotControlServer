namespace RobotControlServer.Services
{
    public class RobotOptions
    {
        public string Host { get; set; } = "192.168.5.1";
        public int Port { get; set; } = 6601;
        public string Encoding { get; set; } = "ASCII";
        public bool AppendNewLine { get; set; } = true;
        public int ConnectTimeoutMs { get; set; } = 3000;
    }
}
