namespace TorClient.Options {
    public class TorOptions {
        public int ProxyPort { get; set; } = 8118;
        public int ControlPort { get; set; } = 9051;
        public object Password { get; set; } = "";
        public int RenewalDelay { get; set; } = 5000;
        public string TorIpAddress { get; set; } = "127.0.0.1";
        public string IpServiceUrl { get; set; } = "http://api.ipify.org";
    }
}