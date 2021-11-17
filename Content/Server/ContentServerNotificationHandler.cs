using Networking;
using System;

namespace Content
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        private ContentServer _contentServer;

        public ContentServerNotificationHandler()
        {
            _contentServer = ContentServerFactory.GetInstance() as ContentServer;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            _contentServer.Receive(data);
        }

        /// <inheritdoc />
        public void OnClientJoined<T>(T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}