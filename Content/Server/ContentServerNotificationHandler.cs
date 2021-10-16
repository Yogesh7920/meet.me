using Networking;
using System;

namespace Content
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        /// <summary>
        /// Handles messages received over the network.
        /// </summary>
        /// <param name="data">received message.</param>
        public void OnDataReceived(string data)
        {
            throw new NotImplementedException();
        }

        public void OnClientJoined<T>(T socketObject)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}