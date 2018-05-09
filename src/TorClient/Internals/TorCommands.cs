namespace TorClient.Internals {
    using System.Diagnostics.CodeAnalysis;

    internal static class TorCommands {

        [SuppressMessage("ReSharper", "InconsistentNaming")] public const string QUIT = "QUIT\n";

        [SuppressMessage("ReSharper", "InconsistentNaming")] public const string AUTHENTICATE = "AUTHENTICATE\n";
        
        // ReSharper disable once IdentifierTypo
        [SuppressMessage("ReSharper", "InconsistentNaming")] public const string RENEWCLIENTCIRCUITS = "SIGNAL NEWNYM\n";
    }
}