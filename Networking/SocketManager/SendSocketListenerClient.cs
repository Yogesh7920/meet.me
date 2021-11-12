using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
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

        // Fix the maximum size of the message that can be sent  one at a time 
        private const int Threshold = 1025;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue</param>
        /// </summary>
        public SendSocketListenerClient(IQueue queue, TcpClient tcpSocket)
        {
            _queue = queue;
            _tcpSocket = tcpSocket;
        }

        /// <summary>
        /// This method is for starting the thread
        /// </summary>
        public void Start()
        {
            _listen = new Thread(Listen);
            _listenRun = true;
            _listen.Start();
        }

        /// <summary>
        /// This method form string from packet object
        /// it also adds EOF to indicate that the message 
        /// that has been popped out from the queue is finished 
        /// </summary>
        ///  /// <returns>String </returns>
        private String GetMessage(Packet packet)
        {
            String msg = packet.ModuleIdentifier;
            msg += ":";
            msg += packet.SerializedData;
            msg += "EOF";
            return msg;
        }

        /// <summary>
        /// This method is for listen to queue and send to server if some packet comes in queue
        /// </summary>
        private void Listen()
        {
            while (_listenRun)
            {
                // If the queue is not empty, get a packet from the front of the queue and remove that packet
                //from the queue
                while (!_queue.IsEmpty())
                {
                    // Dequeue the front packet of the queue
                    Packet packet = _queue.Dequeue();

                    //Call GetMessage function to form string msg from the packet object 
                    String msg = GetMessage(packet);

                    // Variable used in the process of fragmentation
                    String buffer = "";

                    // Send the message in chunks of threshold number of characters, 
                    //if the data size is greater than threshold value
                    for (int i = 0; i < msg.Length; i++)
                    {
                        if (buffer.Length >= Threshold)
                        {
                            // Write the part of the data which is taken from the queue 
                            // to the server socket
                            try
                            {
                                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(buffer);
                                NetworkStream networkStream = _tcpSocket.GetStream();
                                networkStream.Write(outStream, 0, outStream.Length);
                                networkStream.Flush();
                                buffer = "";
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(
                                    "Networking : An Exception has been raised on client in SendSocketListenerClientThread " +
                                    e);
                                return;
                            }
                        }

                        // Append the character of the variable msg at ith position
                        //to the end of the buffer
                        buffer = buffer.Insert(buffer.Length, msg[i].ToString());
                    }

                    // Sending the remaining portion of string after
                    //dividing into chunks of threshold size strings 
                    if (buffer.Length > 0)
                    {
                        try
                        {
                            // Write the part of the data which is taken from the queue 
                            // to the server socket
                            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(buffer);
                            NetworkStream networkStream = _tcpSocket.GetStream();
                            networkStream.Write(outStream, 0, outStream.Length);
                            networkStream.Flush();
                            buffer = "";
                        }
                        catch (Exception e)
                        {
                            // If any exception raises while writing in the socket,
                            // trace the info and stop the thread
                            Trace.WriteLine(
                                "Networking : An  Exception has been raised on client in SendSocketListenerClientThread " +
                                e.ToString());
                            return;
                        }

                        Trace.WriteLine("Message has been sent to server from client");
                    }
                }
            }
        }

        /// <summary>
        /// This method is for stopping the thread
        /// </summary>
        public void Stop()
        {
            _listenRun = false;
        }
    }
}