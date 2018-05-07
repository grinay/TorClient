namespace TorClient {
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using global::TorClient.Abstractions;
    using global::TorClient.Exceptions;
    using global::TorClient.Internals;
    using global::TorClient.Options;

    public class TorClient : ITorClient {
        private string _ipAddress = string.Empty;

        public TorClient(TorOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            Http = new HttpClient(
                new HttpClientHandler {
                    UseProxy = true,
                    Proxy = new WebProxy(
                        options.TorIpAddress,
                        options.ProxyPort)
                }, true);

            TorControl = new TorControlAdapter(this);
        }

        public HttpClient Http { get; }
        public TorOptions Options { get; }
        public ITorControl TorControl { get; }

        public string IpAddress {
            get {
                if (!string.IsNullOrEmpty(_ipAddress)) return _ipAddress;

                return _ipAddress = GetIpAddress().Result;
            }
        }

        public void Dispose() => Http.Dispose();

        protected async Task<string> RenewIpAddress() {
            for (;;) {
                using (var socket = new Socket(
                    socketType: SocketType.Stream, 
                    protocolType: ProtocolType.Tcp,
                    addressFamily: AddressFamily.InterNetwork)) {

                    await socket.ConnectAsync(new IPEndPoint(port: Options.ControlPort,
                        address: IPAddress.Parse(Options.TorIpAddress)));

                    await SendAuthenticate(socket);
                    await SendRenewClientCircuits(socket);
                    await SendQuit(socket);
                }

                await Task.Delay(Options.RenewalDelay);

                var newIpAddress = await GetIpAddress();
                if (!_ipAddress.Equals(newIpAddress, StringComparison.OrdinalIgnoreCase))
                    return _ipAddress = newIpAddress;
            }
        }

        public async Task<string> GetIpAddress() => await Http.GetStringAsync(Options.IpServiceUrl);

        private static async Task SendQuit(Socket socket) => await Send(socket, TorCommands.QUIT);

        private static async Task SendRenewClientCircuits(Socket socket) => await Send(socket, TorCommands.RENEWCLIENTCIRCUITS);

        private async Task SendAuthenticate(Socket socket) => await Send(socket, $"{TorCommands.AUTHENTICATE} \"{Options.Password}\"\n");

        private static async Task Send(Socket socket, string commandText) {
            var buffer = new byte[256];

            var command = Encoding.ASCII.GetBytes($"{commandText}\n");

            await socket.SendAsync(new ArraySegment<byte>(command), SocketFlags.Peek);

            await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.Peek);

            using (var memoryStream = new MemoryStream(buffer))
            using (var streamReader = new StreamReader(memoryStream)) 
                EnsureSuccessStatusCode(await streamReader.ReadLineAsync());
        }

        private static bool IsSuccessStatusCode(string statusCode) {
            return string.Equals(statusCode, "250 OK", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(statusCode, "250 closing connection", StringComparison.OrdinalIgnoreCase);
        }

        private static void EnsureSuccessStatusCode(string statusCode) {
            if (IsSuccessStatusCode(statusCode)) return;

            throw new TorException($"An error occurred during the request: {statusCode}");
        }

        private class TorControlAdapter : ITorControl {
            private readonly TorClient _client;

            public TorControlAdapter(TorClient client) {
                _client = client;
            }

            public void Dispose() {
            }

            public async Task<string> RenewIpAddress() => await _client.RenewIpAddress();
        }
    }
}