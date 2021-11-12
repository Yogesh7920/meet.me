using System;
using System.Reflection;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

/// <summary>
/// This file contains the class Server which will be running on the server.
/// and accepts client request.
/// </summary>
/// <author>Tausif Iqbal </author>
namespace Networking
{
    public class ServerCommunicator : ICommunicator
    {
        /** Declare sendSocketListenerServer variable for sending messages across the network*/
        private SendSocketListenerServer _sendSocketListenerServer;

        private ReceiveQueueListener _receiveQueueListener;

        /** Declare dictionary variable that stores clientId and receiveSocketListener */
        private readonly Dictionary<String, ReceiveSocketListener> _clientListener =
            new();

        /** Declare dictionary variable to store module Id and corresponding handler*/
        private readonly Dictionary<string, INotificationHandler> _subscribedModules =
            new();

        /** Declare dictionary variable to store client Id and corresponding socket object*/
        private readonly Dictionary<string, TcpClient> _clientIdSocket = new();

        /** Declare queue variable for receiving messages*/
        private readonly Queue _receiveQueue = new();

        /** Declare queue variable for sending messages*/
        private readonly Queue _sendQueue = new();

        /** Declare thread variable for accepting request*/
        private Thread _acceptRequest;

        /** Declare variable to control acceptRequest thread*/
        private volatile bool _acceptRequestRun;

        /**Declare TcpListener variable of server*/
        private TcpListener _serverSocket;

        /** Variable for testing mode*/
        private readonly bool _isTesting;

        TcpClient _clientSocket = new();

        /**
         * Constructor that enables testing mode
         */
        public ServerCommunicator(bool isTesting = false)
        {
            _isTesting = isTesting;
        }

        /// <summary>
        /// It finds IP4 address of machine which does not end with .1
        /// </summary>
        /// <returns>IP4 address </returns>
        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string address = ip.ToString();

                    // check  IP does not end with .1 
                    if (address.Split(".")[3] != "1")
                    {
                        return ip.ToString();
                    }
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// scan for free Tcp port.
        /// </summary>
        /// <returns>integer </returns>
        private static int FreeTcpPort(IPAddress ip)
        {
            TcpListener tcp = new TcpListener(ip, 0);
            tcp.Start();
            int port = ((IPEndPoint) tcp.LocalEndpoint).Port;
            tcp.Stop();
            return port;
        }

        /// <summary>
        /// start the server and return ip and port
        /// </summary>
        /// <returns> String</returns>
        string ICommunicator.Start(string serverIp, string serverPort)
        {
            IPAddress ip = IPAddress.Parse(GetLocalIpAddress());
            int port = FreeTcpPort(ip);
            _serverSocket = new TcpListener(ip, port);

            //start server at the scanned port of the ip 
            _serverSocket.Start();

            //start sendSocketListener of server for sending message 
            _sendSocketListenerServer = new SendSocketListenerServer(_sendQueue, _clientIdSocket);
            _sendSocketListenerServer.Start();

            _receiveQueueListener = new ReceiveQueueListener(_receiveQueue, _subscribedModules);
            _receiveQueueListener.Start();

            //start acceptRequest thread of server for accepting request
            _acceptRequest = new Thread(() => AcceptRequest(_serverSocket));
            _acceptRequestRun = true;
            _acceptRequest.Start();

            if (_isTesting)
            {
                TestRegisterModule();
            }

            Trace.WriteLine("Server has started with ip = " + ip.ToString()
                                                            + " and port number = " + port.ToString());

            return (ip.ToString() + ":" + port.ToString());
        }

        /// <summary>
        /// It accepts all incoming client request
        /// </summary>
        /// <returns> void </returns>
        private void AcceptRequest(TcpListener serverSocket)
        {
            // variable used in test mode for assigning client id
            int testCount = 0;

            // Block and wait for incoming client connection
            while (_acceptRequestRun)
            {
                try
                {
                    TcpClient clientSocket = new TcpClient();
                    clientSocket = _serverSocket.AcceptTcpClient();

                    //notify subscribed Module handler
                    foreach (KeyValuePair<string, INotificationHandler> module in _subscribedModules)
                    {
                        module.Value.OnClientJoined(clientSocket);
                    }

                    if (_isTesting)
                    {
                        testCount++;
                        TestAddClient(testCount.ToString(), clientSocket);
                    }
                }
                catch (SocketException e)
                {
                    if ((e.SocketErrorCode == SocketError.Interrupted))
                    {
                        Trace.WriteLine("socket blocking listener has been closed");
                    }
                }
            }
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
        /// This method is for testing purpose
        /// </summary>
        ///  /// <returns> void </returns>
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
        /// </summary>
        ///  /// <returns> void </returns>
        void TestAddClient<T>(string clientId, T socketObject)
        {
            //add clientID and socketObject into Dictionary 
            _clientIdSocket[clientId] = (TcpClient) (Object) socketObject;

            //Start receiveSocketListener of the client in the 
            //server for listening message from the client

            ReceiveSocketListener receiveSocketListener =
                new ReceiveSocketListener(_receiveQueue, (TcpClient) (object) socketObject);
            _clientListener[clientId] = receiveSocketListener;
            receiveSocketListener.Start();
        }

        /// <summary>
        /// closes all the running thread of the server
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Stop()
        {
            //stop acceptRequest thread
            _acceptRequestRun = false;
            _serverSocket.Stop();

            //stop receiveSocketListener of all the clients 
            foreach (KeyValuePair<string, ReceiveSocketListener> listener in _clientListener)
            {
                ReceiveSocketListener receiveSocketListener = listener.Value;
                receiveSocketListener.Stop();
            }

            _receiveQueueListener.Stop();
            // stop sendSocketListener of server
            _sendSocketListenerServer.Stop();
        }

        /// <summary>
        /// It adds client socket to a map and starts listening on that socket
        /// also adds corresponding listener to a set
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.AddClient<T>(string clientId, T socketObject)
        {
            // add clientID and socketObject into Dictionary 
            _clientIdSocket[clientId] = (TcpClient) (Object) socketObject;

            //Start receiveSocketListener of the client in the 
            //server for listening message from the client

            ReceiveSocketListener receiveSocketListener =
                new ReceiveSocketListener(_receiveQueue, (TcpClient) (object) socketObject);
            _clientListener[clientId] = receiveSocketListener;
            receiveSocketListener.Start();
        }

        /// <summary>
        /// It removes client from server 
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.RemoveClient(string clientId)
        {
            // stop the listener of the client 
            ReceiveSocketListener receiveSocketListener = _clientListener[clientId];
            receiveSocketListener.Stop();

            //close stream  and connection of the client
            TcpClient tcpClient = _clientIdSocket[clientId];
            tcpClient.GetStream().Close();
            tcpClient.Close();

            // remove the socket object and listener  of the client 
            _clientListener.Remove(clientId);
            _clientIdSocket.Remove(clientId);
        }

        /// <summary>
        /// It takes data and identifier 
        /// forms packet object  and push to the 
        /// sending queue for broadcast
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
            }
        }

        /// <summary>
        /// It takes data, identifier and destination 
        /// forms packet object and push to the 
        /// sending queue for private messaging
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Send(string data, string identifier, string destination)
        {
            Packet packet = new Packet {ModuleIdentifier = identifier, SerializedData = data, Destination = destination};
            try
            {
                _sendQueue.Enqueue(packet);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// It adds notification handler of module
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Subscribe(string identifier, INotificationHandler handler, int priority)
        {
            _subscribedModules.Add(identifier, handler);
            _receiveQueue.RegisterModule(identifier, priority);
        }
    }
}