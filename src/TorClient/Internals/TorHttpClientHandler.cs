namespace TorClient {

    using global::TorClient.Abstractions;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TorHttpClientHandler : HttpClientHandler {
        private readonly ITorControl _control;

        public TorHttpClientHandler(ITorControl control) {
            _control = control;
            UseProxy = true;
            Proxy = new WebProxy(
                control.Options.TorIpAddress,
                control.Options.ProxyPort);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            await _control.RenewIpAddressAsync()
                .ConfigureAwait(false);

            return response; 
        }
    }
}