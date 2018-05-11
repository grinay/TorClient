namespace TorClient.Abstractions {
    using System;
    using System.Net.Http;

    public interface ITorClient : IDisposable {
        HttpClient Http { get; }
        string IpAddress { get; }
    }
}