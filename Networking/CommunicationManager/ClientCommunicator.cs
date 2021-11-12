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
        private readonly Dictionary<string, INotificationHandler> _subscribedModules = new();

        // Declare socket object for client
        private TcpClient _clientSocket;

        // Declare queue variable for receiving messages
        private readonly Queue _receiveQueue = new();

        // Declare queue variable for sending messages
        private readonly Queue _sendQueue = new();

        // Variable for testing mode
        private readonly bool _isTesting;

        //Constructor that enables testing mode
        public ClientCommunicator(bool isTesting = false)
        {
            _isTesting = isTesting;
        }

        //Declare ReceiveSocketListener variable for listening messages 
        private ReceiveSocketListener _receiveSocketListener;

        // Declare sendSocketListenerClient variable for sending messages 
        private SendSocketListenerClient _sendSocketListenerClient;

        private ReceiveQueueListener _receiveQueueListener;

        /// <summary>
        /// This method connects client to server
        /// <param name="serverIp">serverIP</param>
        /// <param name="serverPort">serverPort.</param>
        /// </summary>
        ///  /// <returns> String </returns>
        string ICommunicator.Start(string serverIp, string serverPort)
        {
            try
            {
                //try to connect with server
                IPAddress ip = IPAddress.Parse(serverIp);
                int port = Int32.Parse(serverPort);
                _clientSocket = new TcpClient();
                // _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
                _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                _clientSocket.Connect(ip, port);

                //Start receiveSocketListener of the client  
                //for listening message from the server
                _receiveSocketListener = new ReceiveSocketListener(_receiveQueue, _clientSocket);
                _receiveSocketListener.Start();

                //start sendSocketListener of client for sending message 
                _sendSocketListenerClient = new SendSocketListenerClient(_sendQueue, _clientSocket);
                _sendSocketListenerClient.Start();

                _receiveQueueListener = new ReceiveQueueListener(_receiveQueue, _subscribedModules);
                _receiveQueueListener.Start();

                //for testing purpose
                if (_isTesting)
                {
                    TestRegisterModule();
                }

                return "1";
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return "0";
            }
        }

        /// <summary>
        /// This method is for testing purpose
        /// </summary>
        /// <returns> void </returns>
        void TestRegisterModule()
        {
            _sendQueue.RegisterModule("S", 1);
            _sendQueue.RegisterModule("W", 2);
            _sendQueue.RegisterModule("C", 3);
            _sendQueue.RegisterModule("F", 4);
            _receiveQueue.RegisterModule("S", 1);
            _receiveQueue.RegisterModule("W", 2);
            _receiveQueue.RegisterModule("C", 3);
            _receiveQueue.RegisterModule("F", 4);
        }

        /// <summary>
        /// This method is for testing purpose
        /// and can be called in testing mode
        /// </summary>
        /// <returns> packet </returns>
        public Packet FrontPacket()
        {
            if (_isTesting)
            {
                Packet packet = new Packet();
                if (_receiveQueue.Size() != 0)
                {
                    packet = _receiveQueue.Dequeue();
                }

                return packet;
            }

            throw new Exception("You don't have access");
        }

        /// <summary>
        /// This method stops all the running thread
        ///  of client and closes the connection
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Stop()
        {
            if (_clientSocket.Connected)
            {
                // stop the listener of the client 
                _sendSocketListenerClient.Stop();
                _receiveSocketListener.Stop();
                _receiveQueueListener.Stop();

                //close stream  and connection of the client
                _clientSocket.GetStream().Close();
                _clientSocket.Close();
            }
        }

        /// <inheritdoc />
        void ICommunicator.AddClient<T>(string clientId, T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        void ICommunicator.RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is for sending message
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Send(string data, string identifier)
        {
            Packet packet = new Packet {ModuleIdentifier = identifier, SerializedData = data};
            try
            {
                _sendQueue.Enqueue(packet);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        void ICommunicator.Send(string data, string identifier, string destination)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void ICommunicator.Subscribe(string identifier, INotificationHandler handler, int priority)
        {
            _subscribedModules.Add(identifier, handler);
            _sendQueue.RegisterModule(identifier, priority);
            _receiveQueue.RegisterModule(identifier, priority);
        }
    }
}