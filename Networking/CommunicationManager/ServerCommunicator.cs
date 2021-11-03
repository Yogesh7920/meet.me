using System;
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
        private Dictionary<string, INotificationHandler> _subscribedModules = new Dictionary<string, INotificationHandler>();
        private Hashtable _clientIdSocket = new Hashtable();
        private Queue _recieveQueue = new Queue();

        private Thread _acceptRequest;
        private bool _acceptRequestRun = false;
        private SendSocketListenerServer _sendSocketListenerServer;
       
       private HashSet<RecieveSocketListener> clientListener = new HashSet<RecieveSocketListener>();

        /// <summary>
        /// It finds IP4 address of machine which does not end with .1
        /// </summary>
        /// <returns>IP4 address </returns>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string address = ip.ToString();
                    int n = address.Length;
                    if( address[n-1]!='1' && address[n - 2] != '.')
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
            
            Trace.WriteLine("Server has started with ip = " +
            IPAddress.Parse(((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString()) +
            " and port number = " + ((IPEndPoint)serverSocket.LocalEndpoint).Port.ToString());
            
            _sendSocketListenerServer = new SendSocketListenerServer(_recieveQueue, _clientIdSocket); 
            _sendSocketListenerServer.Start();
            _acceptRequest = new Thread(() => AcceptRequest(serverSocket));
            _acceptRequestRun = true;
            _acceptRequest.Start();

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
            while (_acceptRequestRun)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                foreach (KeyValuePair<string, INotificationHandler> module in _subscribedModules)
                {
                    module.Value.OnClientJoined(clientSocket);
                }
            }
        }

        /// <summary>
        /// closes all the running thread of the server
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.Stop()
        {
            _acceptRequestRun = false;
            // run a loop and stop  recieveSocketListener of all the clients
            foreach( RecieveSocketListener r in clientListener)
            {
                r.Stop();
            }
            _sendSocketListenerServer.Stop();
            
        }

        /// <summary>
        /// It adds client socket to a map and starts listening on that socket
        /// also adds corresponding listener to a set
        /// </summary>
        /// <returns> void </returns>
        void ICommunicator.AddClient<T>(string clientID, T socketObject) 
        {
            _clientIdSocket[clientID] = socketObject;
            RecieveSocketListener recieveSocketListener =new RecieveSocketListener(_recieveQueue, (TcpClient)(object)socketObject);
            clientListener.Add(recieveSocketListener);
            recieveSocketListener.Start();
            

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
            _subscribedModules.Add(identifier, handler);
        }
        
    }
}
