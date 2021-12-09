/// <author>Tausif Iqbal</author>
/// <created>13/10/2021</created>
/// <summary>
/// This file contains the class definition of SendSocketListenerServer.
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking
{
    public class SendSocketListenerServer
    {
        // Declare the dictionary variable which stores client_ID and corresponding socket object 
        private readonly Dictionary<string, TcpClient> _clientIdSocket;

        // Declare the queue variable which is used to dequeue the required the packet 
        private readonly IQueue _queue;

        // Declare dictionary variable to get handler
        private readonly Dictionary<string, INotificationHandler> _subscribedModules;

        private bool _isTesting;

        // Declare the thread variable of SendSocketListenerServer 
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        /// <summary>
        ///     This is the constructor of the class which initializes the params
        ///     <param name="queue">queue</param>
        /// </summary>
        public SendSocketListenerServer(IQueue queue, Dictionary<string, TcpClient> clientIdSocket,
            Dictionary<string, INotificationHandler> subscribedModules)
        {
            _queue = queue;
            _clientIdSocket = clientIdSocket;
            _subscribedModules = subscribedModules;
        }

        /// <summary>
        ///     This method is for starting the thread
        /// </summary>
        /// <returns> Void  </returns>
        public void Start()
        {
            _listen = new Thread(Listen);
            _listenRun = true;
            _listen.Start();
            var testMode = Environment.GetEnvironmentVariable("TEST_MODE");
            _isTesting = testMode is "MODULE" or "UNIT";
            Trace.WriteLine("[Networking] SendSocketListenerServer thread started.");
        }

        /// <summary>
        ///     This method extracts destination from packet
        ///     if destination is null , then it is case of broadcast so
        ///     it returns all the client socket objects
        ///     else it returns only the socket of that client
        /// </summary>
        /// <param name="packet">Packet Object.</param>
        /// <returns> HashSet object containing tcpClient </returns>
        private HashSet<TcpClient> GetDestination(Packet packet)
        {
            var tcpSocket = new HashSet<TcpClient>();

            // check packet contains destination or not
            if (packet.Destination == null)
            {
                foreach (var tcpClient in _clientIdSocket) tcpSocket.Add(tcpClient.Value);
            }
            else
            {
                var clientId = packet.Destination;
                tcpSocket.Add(_clientIdSocket[clientId]);
            }

            return tcpSocket;
        }

        /// <summary>
        ///     This method extracts client Id
        ///     from tcpSocket Object
        /// </summary>
        /// <param name="tcpSocket">TcpClient Object</param>
        /// <returns> String </returns>
        private string GetClientId(TcpClient tcpSocket)
        {
            foreach (var clientId in _clientIdSocket.Keys)
                if (_clientIdSocket[clientId] == tcpSocket)
                    return clientId;

            return null;
        }

        /// <summary>
        ///     This method is for listening  to queue and for sending to server if some packet comes in queue
        /// </summary>
        /// <returns> Void  </returns>
        private void Listen()
        {
            while (_listenRun)
            {
                _queue.WaitForPacket();
                // If the queue is not empty, get a packet from the front of the queue and remove that packet
                // from the queue
                while (_queue.Size() != 0)
                {
                    // Dequeue the front packet of the queue
                    var packet = _queue.Dequeue();

                    // Call GetMessage function to form string msg from the packet object 
                    var msg = Utils.GetMessage(packet);

                    // Call GetDestination function to know destination from the packet object
                    var tcpSockets = GetDestination(packet);

                    foreach (var tcpSocket in tcpSockets)
                    {
                        var outStream = Encoding.ASCII.GetBytes(msg);
                        try
                        {
                            var socket = tcpSocket.Client;

                            // check client is still connected or not
                            if (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0)
                            {
                                Trace.WriteLine("[Networking] Client lost connection! Retrying...");

                                // client is disconnected try 3 times to send data
                                _ = Task.Run(() =>
                                {
                                    var tcpSocketTry = tcpSocket;
                                    var outStreamTry = outStream;
                                    var isSent = false;
                                    var sTry = tcpSocketTry.Client;
                                    try
                                    {
                                        for (var t = 0; t < 3; t++)
                                        {
                                            Thread.Sleep(_isTesting ? 1 : 100);
                                            if (!(sTry.Poll(1, SelectMode.SelectRead) && sTry.Available == 0))
                                            {
                                                Trace.WriteLine("[Networking] Client has reconnected!");
                                                sTry.Send(outStreamTry);
                                                Trace.WriteLine(
                                                    $"[Networking] Data sent from server to client by {packet.ModuleIdentifier}.");
                                                isSent = true;
                                                break;
                                            }
                                        }

                                        if (isSent == false)
                                        {
                                            Trace.WriteLine("[Networking] Client has disconnected! Removing Client...");
                                            var clientId = GetClientId(tcpSocketTry);

                                            // call notification handler for removing the client
                                            foreach (var module in
                                                _subscribedModules)
                                                if (clientId != null)
                                                    module.Value.OnClientLeft(clientId);
                                                else
                                                    Trace.WriteLine("[Networking] ClientId is not present");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Trace.WriteLine($"[Networking] {e.Message}");
                                    }
                                });
                            }
                            else
                            {
                                socket.Send(outStream);
                                Trace.WriteLine(
                                    $"[Networking] Data sent from server to client by {packet.ModuleIdentifier}.");
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine($"[Networking] {e.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This method is for stopping the thread
        /// </summary>
        /// ///
        /// <returns> Void  </returns>
        public void Stop()
        {
            _listenRun = false;
            Trace.WriteLine("[Networking] Stopped SendSocketListenerServer thread.");
        }
    }
}
