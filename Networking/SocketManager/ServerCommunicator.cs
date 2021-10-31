using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// This file contains the class Server which will be running on the server.
/// and accepts client request.
/// </summary>
/// <author>Tausif Iqbal </author>
namespace Networking
{
    public class ServerCommunicator : ICommunicator
    {
        private Dictionary<string, INotificationHandler> subscribedModules = new Dictionary<string, INotificationHandler>();
        private Hashtable clientIdSocket = new Hashtable();
        private Queue _globalQueue = new Queue();

        private Thread acceptRequest;
        private Thread sendQueueListener;
        /// <summary>
        /// It finds IP4 address of machine
        /// </summary>
        /// <returns>IP4 address </returns>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        
        /// <summary>
        /// scan for free Tcp port.
        /// </summary>
        /// <returns>Free port </returns>
        private static int FreeTcpPort()
        {
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            TcpListener tcp = new TcpListener(ip, 0);
            tcp.Start();
            int port = ((IPEndPoint)tcp.LocalEndpoint).Port;
            tcp.Stop();
            return port;
        }
        
        /// <summary>
        /// start the server and return ip and port
        /// </summary>
        /// <returns> String</returns>
        string ICommunicator.Start(string serverIp, string serverPort)
        {
            int port = FreeTcpPort();
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            TcpListener serverSocket = new TcpListener(ip, port);
            serverSocket.Start();
            TcpClient clientSocket = default(TcpClient);
            Console.WriteLine("Server has started with ip = " +
            IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
            " and port number = " + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());
            
            sendQueueListener = new SendQueueListener(_globalQueue);; 
            sendQueueListener.Start();
            acceptRequest = new Thread(() => AcceptRequest(serverSocket));
            acceptRequest.Start();

            return IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString())+ 
                    ":" + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString();
        }

        /// <summary>
        /// It accepts all incoming client request
        /// </summary>
        /// <returns> void </returns>
        private void AcceptRequest(TcpListener serverSocket)
        {
            TcpClient clientSocket = default(TcpClient);
            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                onClientJoined(clientSocket);
            }
        }

        /// <summary>
        /// closes all the running thread of the server
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Stop()
        {
            sendQueueListener.Abort();
            acceptRequest.Abort();

        }
        /// <summary>
        /// It adds client socket to a map and starts listening on that socket
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.AddClient<T>(string clientID, T socketObject)
        {
            ClientIdSocket[ClientID] = socketObject;
            SocketListener socketListener =new SocketListener(_globalQueue,socketObject);
            socketListener.Start();
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
