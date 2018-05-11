namespace TorClient {
    using global::TorClient.Abstractions;
    using global::TorClient.Options;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class TorClient : ITorClient {

        private readonly TorOptions _options;
        private readonly Lazy<HttpClient> _httpClient;
        private readonly HttpMessageHandler _httpMessageHandler;

        public TorClient(ITorControl control) {
            _options = control.Options;

            _httpMessageHandler = new TorHttpClientHandler(control);

            _httpClient = new Lazy<HttpClient>(() => 
                new HttpClient(_httpMessageHandler));
        }

        public HttpClient Http => _httpClient.Value;

        public string IpAddress =>
            GetIpAddressAsync()
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();

        public void Dispose() {
            _httpMessageHandler.Dispose();

            if (!_httpClient.IsValueCreated) return;

            _httpClient.Value.Dispose();
        }

        private Task<string> GetIpAddressAsync() =>
            Http.GetStringAsync(_options.IpServiceUrl);
    }
     
}