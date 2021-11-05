using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Networking
{
    public class SendSocketListenerClient
    {
        // It contains the packets which needs to be sent over the network
        private IQueue _queue;
        private TcpClient _tcpSocket;
        private Thread _listen;
        private volatile bool _listenRun = false;
        const int _threshold = 1025;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue</param>
        /// </summary>
        public SendSocketListenerClient(IQueue queue, TcpClient tcpsocket)
        {
            this._queue = queue;
            this._tcpSocket = tcpsocket;
        }

        /// <summary>
        /// This method is for starting the thread
        /// </summary>
        public void Start()
        {
            _listen = new Thread(() => Listen());
            _listenRun = true;
            _listen.Start();
        }

        /// <summary>
        /// This method form string from packet object
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
                // dequeue from queue and write to network Stream
                while (!_queue.IsEmpty())
                {
                    Packet packet = _queue.Dequeue();
                    // call get message function to form string from packet
                    String msg = GetMessage(packet);
                    String buffer = "";
                    for (int i = 0; i < msg.Length; i++)
                    {
                        if (buffer.Length >= _threshold)
                        {
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
                                Trace.WriteLine(e);
                                return;
                            }
                        }

                        buffer = buffer.Insert(buffer.Length, msg[i].ToString());
                    }

                    if (buffer.Length > 0)
                    {
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
                            Trace.WriteLine(e);
                            return;
                        }
                    }
                }

                Trace.WriteLine("Message has been sent to server from client");
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