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
        
        /// <summary>
        /// scan for free Tcp port.
        /// </summary>
        /// <returns>Free port </returns>
        private static int FreeTcpPort()
        {
            TcpListener tcp = new TcpListener(System.Net.IPAddress.Loopback, 0);
            tcp.Start();
            int port = ((IPEndPoint)tcp.LocalEndpoint).Port;
            tcp.Stop();
            return port;
        }
        
        string ICommunicator.Start(string serverIp, string serverPort)
        {
            int port = FreeTcpPort();
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Loopback, port);
            serverSocket.Start();
            TcpClient clientSocket = default(TcpClient);
            Console.WriteLine("Server has started with ip = " +
            IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
            " and port number = " + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());

            return IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString())+ 
                    ":" + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString();
        }
        
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
