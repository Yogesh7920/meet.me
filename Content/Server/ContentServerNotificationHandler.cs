using Networking;
using System;

namespace Content
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        private IContentServer contentServer;

        public ContentServerNotificationHandler()
        {
            contentServer = ContentServerFactory.GetInstance();
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            contentServer.Receive(data);
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