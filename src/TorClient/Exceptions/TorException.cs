namespace TorClient.Exceptions {
    using System;

    [Serializable]
    public class TorException : Exception {
        public TorException(string message) : base(message) {
        }
    }
}