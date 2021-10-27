using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

/// <summary>
/// This file contains the class Server which will be running on the server.
/// and accepts client request.
/// </summary>
/// <author>Tausif Iqbal </author>
namespace Networking
{
    public class Server : ICommunicator
    {
        public void ICommunicator.Start(string serverIp = null, string serverPort = null)
        {
            int port = FreeTcpPort();
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Loopback, port);
            serverSocket.Start();
            TcpClient clientSocket = default(TcpClient);
            Console.WriteLine("Server has started with ip = " +
            IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
            " and port number = " + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
            }

        }

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
        
    }
}
