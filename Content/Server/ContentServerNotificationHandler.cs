using Networking;
using System;

namespace Content
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            throw new NotImplementedException();
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