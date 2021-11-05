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
        private Dictionary<string, INotificationHandler> subscribedModules =
            new Dictionary<string, INotificationHandler>();

        private TcpClient _clientSocket = new TcpClient();
        private Queue _queue = new Queue();
        private RecieveSocketListener recieveSocketListener;
        private SendSocketListenerClient sendSocketListenerClient;

        /// <summary>
        /// This method connects client to server
        /// <param name="serverIP">serverIP</param>
        /// <param name="serverPort">serverPort.</param>
        /// </summary>
        ///  /// <returns> String </returns>
        string ICommunicator.Start(string serverIP, string serverPort)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(serverIP);
                int port = Int32.Parse(serverPort);
                _clientSocket.Connect(ip, port);

                recieveSocketListener = new RecieveSocketListener(_queue, _clientSocket);
                recieveSocketListener.Start();
                sendSocketListenerClient = new SendSocketListenerClient(_queue, _clientSocket);
                sendSocketListenerClient.Start();
                return "1";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "0";
            }
        }

        /// <inheritdoc />
        void ICommunicator.Stop()
        {
            sendSocketListenerClient.Stop();
            recieveSocketListener.Stop();
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