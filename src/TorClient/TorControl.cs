namespace TorClient {
    using System.Threading.Tasks;
    using global::TorClient.Abstractions;
    using global::TorClient.Options;

    public class TorControl : TorClient, ITorControl {
        public TorControl(TorOptions options) : base(options) {
        }

        public Task RenewIpAddressAsync() => RenewIpAddress();
    }
}