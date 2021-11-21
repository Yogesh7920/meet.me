/// <author>Tausif Iqbal</author>
/// <created>14/10/2021</created>
/// <modified>16/11/202</modified>
/// <summary>
///     This file contains the class definition
///     of SendSocketListenerClient
/// </summary>

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Networking
{
    public class SendSocketListenerClient
    {
        // Fix the maximum size of the message that can be sent  one at a time 
        private const int Threshold = 1025;

        // Declare the queue variable which is used to dequeue the required the packet 
        private readonly IQueue _queue;

        // Declare the TcpClient  variable 
        private readonly TcpClient _tcpSocket;

        // Declare the thread variable of SendSocketListenerServer 
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        /// <summary>
        ///     This method is the constructor of the class which initializes the params
        ///     <param name="queue">queue</param>
        /// </summary>
        public SendSocketListenerClient(IQueue queue, TcpClient tcpSocket)
        {
            _queue = queue;
            _tcpSocket = tcpSocket;
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
        ///     that has been popped out from the queue is finished
        /// </summary>
        /// ///
        /// <returns>String </returns>
        private static string GetMessage(Packet packet)
        {
            var msg = packet.ModuleIdentifier;
            msg += ":";
            msg += packet.SerializedData;
            msg += "EOF";
            return msg;
        }

        /// <summary>
        ///     This method is for listen to queue and send to server if some packet comes in queue
        /// </summary>
        private void Listen()
        {
            while (_listenRun)
            {
                // If the queue is not empty, get a packet from the front of the queue
                // and remove that packet from the queue
                while (!_queue.IsEmpty())
                {
                    // Dequeue the front packet of the queue
                    var packet = _queue.Dequeue();

                    //Call GetMessage function to form string msg from the packet object 
                    var msg = GetMessage(packet);
                    // Send the message in chunks of threshold number of characters, 
                    // if the data size is greater than threshold value
                    for (var i = 0; i < msg.Length; i += Threshold)
                    {
                        var chunk = msg[i..Math.Min(msg.Length, i + Threshold)];
                        var outStream = Encoding.ASCII.GetBytes(chunk);
                        try
                        {
                            var networkStream = _tcpSocket.GetStream();
                            networkStream.Write(outStream, 0, outStream.Length);
                            networkStream.Flush();
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(
                                "Networking: An Exception has been raised in SendSocketListenerClientThread "
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