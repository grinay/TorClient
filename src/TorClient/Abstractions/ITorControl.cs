namespace TorClient.Abstractions {
    using System;
    using System.Threading.Tasks;

    public interface ITorControl : IDisposable {
        Task RenewIpAddress();
    }
}