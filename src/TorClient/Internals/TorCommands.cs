namespace TorClient.Internals {
    internal class TorCommands {
        public const string QUIT = "QUIT\n";
        public const string AUTHENTICATE = "AUTHENTICATE\n";
        public const string RENEWCLIENTCIRCUITS = "SIGNAL NEWNYM\n";
    }
}