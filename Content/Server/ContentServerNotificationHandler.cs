using Networking;

namespace Content
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        private readonly ContentServer _contentServer;

        internal ContentServerNotificationHandler(ContentServer contentServer)
        {
            _contentServer = contentServer;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            _contentServer.Receive(data);
        }
    }
}