using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Networking
{
    public class ClientCommunicator : ICommunicator
    {
        private Dictionary<string, INotificationHandler> subscribedModules = new Dictionary<string, INotificationHandler>();
        private TcpClient _clientSocket = new TcpClient();
        private Queue _queue = new Queue();
        private RecieveSocketListener recievesocketListener;

        /// <summary>
        /// This method connects client to server
        /// <param name="serverIP">serverIP</param>
        /// <param name="serverPort">serverPort.</param>
        /// </summary>
        string ICommunicator.Start(string serverIP, string serverPort)
        {
            IPAddress ip = IPAddress.Parse(serverIP);
            int port = Int32.Parse(serverPort);
            _clientSocket.Connect(ip, port);

            if (_clientSocket.Connected)
            {
                recievesocketListener = new RecieveSocketListener(_queue, _clientSocket);
                Thread s = new Thread(recievesocketListener.Start);
                s.Start();
                return "1";
            }
            return "0";
        }
        /// <inheritdoc />
        void ICommunicator.Stop()
        {
            recievesocketListener.Stop();
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
            Packet packet = new Packet {ModuleIdentifier = identifier, SerializedData = data};
            try
            {
                _queue.Enqueue(packet);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            
            throw new NotImplementedException();
        }
        /// <inheritdoc />
        void ICommunicator.Send(string data, string identifier, string destination)
        {
            
            throw new NotSupportedException();
        }
        /// <inheritdoc />
        void ICommunicator.Subscribe(string identifier, INotificationHandler handler)
        {
            subscribedModules.Add(identifier, handler);
        }
    }
}
