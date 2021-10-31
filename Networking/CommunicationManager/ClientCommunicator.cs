using System;
using System.Collections.Generic;

namespace Networking
{
    public class ClientCommunicator : ICommunicator
    {
        private Dictionary<string, INotificationHandler> subscribedModules = new Dictionary<string, INotificationHandler>();
        private TcpClient _clientSocket = new TcpClient();
        private Queue _queue = new Queue();

        /// <summary>
        /// This method connects client to server
        /// <param name="serverIP">serverIP</param>
        /// <param name="serverPort">serverPort.</param>
        /// </summary>
        string ICommunicator.Start(string serverIP, string serverPort)
        {
            _clientSocket.Connect(serverIP, serverPort);
            SocketListener socketListener =new SocketListener(_queue,_clientSocket);
            socketListener.Start();
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
