using System;
using System.Collections.Generic;

namespace Networking
{
    public class ClientCommunicator : ICommunicator
    {
        private Dictionary<string, INotificationHandler> subscribedModules = new Dictionary<string, INotificationHandler>();
        /// <inheritdoc />
        string ICommunicator.Start(string serverIP, string serverPort)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.Stop()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.AddClient<T>(string clientID, T socketObject)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.RemoveClient(string clientID)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.Send(string data, string identifier)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.Send(string data, string identifier, string destination)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.Subscribe(string identifier, INotificationHandler handler)
        {
            subscribedModules.Add(identifier, handler);
        }
    }
}
