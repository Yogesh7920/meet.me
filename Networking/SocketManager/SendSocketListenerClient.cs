/// <author>Tausif Iqbal</author>
/// <created>13/10/2021</created>
/// <summary>
/// This file contains the class definition of SendSocketListenerClient.
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
        /// <returns> Void  </returns>
        public void Start()
        {
            _listen = new Thread(Listen);
            _listenRun = true;
            _listen.Start();
            Trace.WriteLine("[Networking] SendSocketListenerClient thread started.");
        }

        /// <summary>
        ///     This method is for listen to queue and send to server if some packet comes in queue
        /// </summary>
        /// <returns> Void  </returns>
        private void Listen()
        {
            while (_listenRun)
            {
                _queue.WaitForPacket();
                // If the queue is not empty, get a packet from the front of the queue
                // and remove that packet from the queue
                while (!_queue.IsEmpty())
                {
                    // Dequeue the front packet of the queue
                    var packet = _queue.Dequeue();

                    //Call GetMessage function to form string msg from the packet object 
                    var msg = Utils.GetMessage(packet);
                    var outStream = Encoding.ASCII.GetBytes(msg);
                    try
                    {
                        _tcpSocket.Client.Send(outStream);
                        Trace.WriteLine($"[Networking] Data sent from client to server by {packet.ModuleIdentifier}.");
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(
                            "[Networking] An Exception has been raised in SendSocketListenerClientThread "
                            + e.Message);
                    }
                }
            }
        }

        /// <summary>
        ///     This method is for stopping the thread
        /// </summary>
        /// <returns> Void  </returns>
        public void Stop()
        {
            _listenRun = false;
            Trace.WriteLine("[Networking] Stopped SendSocketListenerClient thread.");
        }
    }
}
