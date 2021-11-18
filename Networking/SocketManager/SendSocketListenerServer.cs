using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking
{
    public class SendSocketListenerServer
    {
        // Fix the maximum size of the message that can be sent  one at a time
        private const int Threshold = 1025;

        // Declare the dictionary variable which stores client_ID and corresponding socket object
        private readonly Dictionary<string, TcpClient> _clientIdSocket;

        // Declare the queue variable which is used to dequeue the required the packet
        private readonly IQueue _queue;

        // Declare the thread variable of SendSocketListenerServer
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        /// <summary>
        ///     This is the constructor of the class which initializes the params
        ///     <param name="queue">queue</param>
        /// </summary>
        public SendSocketListenerServer(IQueue queue, Dictionary<string, TcpClient> clientIdSocket)
        {
            _queue = queue;
            _clientIdSocket = clientIdSocket;
        }

        /// <summary>
        ///     This method is for starting the thread
        /// </summary>
        public void Start()
        {
            _listen = new Thread(Listen);
            _listenRun = true;
            _listen.Start();
        }

        /// <summary>
        ///     This method form string from packet object
        ///     it also adds EOF to indicate that the message
        ///     that has been popped out feom the queue is finished
        /// </summary>
        /// ///
        /// <returns>String </returns>
        private string GetMessage(Packet packet)
        {
            var msg = packet.ModuleIdentifier;
            msg += ":";
            msg += packet.SerializedData;
            msg += "EOF";
            return msg;
        }

        /// <summary>
        ///     This method extract destination from packet
        ///     if destination is null , then it is case of broadcast so
        ///     it returns all the client socket objects
        ///     else it returns only the socket of that client
        /// </summary>
        /// <returns> a set of socket object  </returns>
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
        ///     This method is for listen to queue and send to server if some packet comes in queue
        /// </summary>
        private void Listen()
        {
            while (_listenRun)
                // If the queue is not empty, get a packet from the front of the queue and remove that packet
                // from the queue
            while (_queue.Size() != 0)
            {
                // Dequeue the front packet of the queue
                var packet = _queue.Dequeue();

                // Call GetMessage function to form string msg from the packet object
                var msg = GetMessage(packet);

                // Call GetDestination function to know destination from the packet object
                var tcpSockets = GetDestination(packet);

                // Send the message in chunks of threshold number of characters,
                // if the data size is greater than threshold value
                for (var i = 0; i < msg.Length; i += Threshold)
                {
                    var chunk = msg[i..Math.Min(msg.Length, i + Threshold)];
                    foreach (var tcpSocket in tcpSockets)
                    {
                        var outStream = Encoding.ASCII.GetBytes(chunk);
                        try
                        {
                            var networkStream = tcpSocket.GetStream();
                            networkStream.Write(outStream, 0, outStream.Length);
                            networkStream.Flush();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(
                                "Networking: Error in SendSocketListenerServerThread "
                                + e.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This method is for stopping the thread
        /// </summary>
        public void Stop()
        {
            _listenRun = false;
        }
    }
}