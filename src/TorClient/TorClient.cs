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
        private readonly Lazy<HttpClient> _httpClient;

        public TorClient(TorOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            _httpClient = new Lazy<HttpClient>(() =>
                new HttpClient(
                    new HttpClientHandler {
                        UseProxy = true,
                        Proxy = new WebProxy(
                            options.TorIpAddress,
                            options.ProxyPort)
                    }, true));

            TorControl = new TorControlAdapter(this);
        }

        public TorOptions Options { get; }
        public ITorControl TorControl { get; }
        public HttpClient Http => _httpClient.Value;
        public string IpAddress => GetIpAddressAsync().Result;

        public void Dispose() {
            if (!_httpClient.IsValueCreated) return;

            Http.Dispose();
        }

        protected async Task RenewIpAddress() {
            using (var socket = new Socket(
                socketType: SocketType.Stream,
                protocolType: ProtocolType.Tcp,
                addressFamily: AddressFamily.InterNetwork)) {
                await socket.ConnectAsync(new IPEndPoint(port: Options.ControlPort,
                    address: IPAddress.Parse(Options.TorIpAddress))).ConfigureAwait(false);

                await SendAuthenticateAsync(socket).ConfigureAwait(false);
                await SendRenewClientCircuitsAsync(socket).ConfigureAwait(false);
                await SendQuitAsync(socket).ConfigureAwait(false);
            }
        }

        private Task<string> GetIpAddressAsync() => Http.GetStringAsync(Options.IpServiceUrl);

        private static Task SendQuitAsync(Socket socket) => SendAsync(socket, TorCommands.QUIT);

        private static Task SendRenewClientCircuitsAsync(Socket socket) =>
            SendAsync(socket, TorCommands.RENEWCLIENTCIRCUITS);

        private Task SendAuthenticateAsync(Socket socket) =>
            SendAsync(socket, $"{TorCommands.AUTHENTICATE} \"{Options.Password}\"\n");

        private static async Task SendAsync(Socket socket, string commandText) {
            var buffer = new byte[256];

            var command = Encoding.ASCII.GetBytes($"{commandText}\n");

            await socket.SendAsync(new ArraySegment<byte>(command), SocketFlags.Peek).ConfigureAwait(false);

            await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.Peek).ConfigureAwait(false);

            using (var memoryStream = new MemoryStream(buffer))
            using (var streamReader = new StreamReader(memoryStream))
                EnsureSuccessStatusCode(await streamReader.ReadLineAsync().ConfigureAwait(false));
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
                /* nothing to do here. */
            }

            public Task RenewIpAddressAsync() => _client.RenewIpAddress();
        }
    }
}