namespace TorClient.Abstractions {

    using System.Threading.Tasks;
    using global::TorClient.Options;

    public interface ITorControl {

        TorOptions Options { get; }

        Task RenewIpAddressAsync();
    }
}