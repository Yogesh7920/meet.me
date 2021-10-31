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
    public class ServerCommunicator : ICommunicator
    {
        public string ICommunicator.Start(string serverIp = null, string serverPort = null)
        {
            int port = FreeTcpPort();
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            TcpListener serverSocket = new TcpListener(ip, port);
            serverSocket.Start();
            TcpClient clientSocket = default(TcpClient);
            Console.WriteLine("Server has started with ip = " +
            IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
            " and port number = " + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());

            return IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString())+ 
                    ":" + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString();
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
        /// It finds IP4 address of machine
        /// </summary>
        /// <returns>IP4 address </returns>
        public static string GetLocalIPAddress()
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
        
    }
}
