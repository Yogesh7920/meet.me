/// <author>Tausif Iqbal</author>
/// <created>14/10/2021</created>
/// <modified>16/11/202</modified>

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Networking
{
    public class ReceiveSocketListener
    {
        // Fix the maximum size of the message that can be sent  one at a time 
        private const int Threshold = 1025;

        // Declare the TcpClient  variable 
        private readonly TcpClient _clientSocket;

        // Declare the queue variable which is used to dequeue the required the packet 
        private readonly IQueue _queue;

        // Declare the thread variable of ReceiveSocketListener 
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        /// <summary>
        ///     This is the constructor of the class which initializes the params
        ///     <param name="queue">An object of type IQueue that is used by the communicator</param>
        ///     <param name="clientSocket">The socket object of the connection</param>
        /// </summary>
        public ReceiveSocketListener(IQueue queue, TcpClient clientSocket)
        {
            _queue = queue;
            _clientSocket = clientSocket;
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
        ///     This forms packet object out of received string
        ///     it looks for EOF to know the end of message
        /// </summary>
        /// <returns>Packet </returns>
        private static Packet GetPacket(string[] msg)
        {
            var packet = new Packet
            {
                ModuleIdentifier = msg[0]
            };

            //get serialized data of packet
            var data = string.Join(":", msg[1..]);

            // search of EOF to get  end of the message
            packet.SerializedData = data[..data.LastIndexOf("EOF", StringComparison.Ordinal)];
            return packet;
        }

        /// <summary>
        ///     This method runs on a thread and listen for incoming message
        /// </summary>
        private void Listen()
        {
            //Variable to store the entire message
            var message = "";
            while (_listenRun)
                try
                {
                    //Get NetworkStream to read message
                    var networkStream = _clientSocket.GetStream();

                    //read when data is available into a buffer
                    while (networkStream.DataAvailable)
                    {
                        var inStream = new byte[Threshold];
                        networkStream.Read(inStream, 0, inStream.Length);
                        var buffer = Encoding.ASCII.GetString(inStream);
                        for (var i = 0; i < Threshold; i++)
                            if (buffer[i] != '\u0000')
                            {
                                message += buffer[i];
                                if (!message.Contains("EOF")) continue;
                                //Calls GetPacket method to form packet object out of received message
                                var packet = GetPacket(message.Split(":"));
                                //Calls the PushToQueue method to push packet into queue
                                PushToQueue(packet.SerializedData, packet.ModuleIdentifier);
                                message = "";
                            }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(
                        "Networking: An Exception has been raised in ReceiveSocketListenerClientThread "
                        + ex.Message);
                }
        }

        /// <summary>
        ///     This method closes the listen thread
        /// </summary>
        public void Stop()
        {
            _listenRun = false;
        }

        /// <summary>
        ///     This method is for pushing the data into the queue
        /// </summary>
        private void PushToQueue(string data, string moduleIdentifier)
        {
            var packet = new Packet { ModuleIdentifier = moduleIdentifier, SerializedData = data };
            Trace.WriteLine("SERVER/CLIENT : " + data);
            _queue.Enqueue(packet);
        }
    }
}