using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.Diagnostics;

/// <summary>
/// This file contains the implementation of socketListener
/// socketListener will continously listen for message and
/// after getting the message it will push into queue 
/// </summary>
/// <author>Tausif Iqbal </author>
namespace Networking
{
    public class ReceiveSocketListener
    {
        // Declare the queue variable which is used to dequeue the required the packet 
        private readonly IQueue _queue;

        // Declare the thread variable of ReceiveSocketListener 
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        // Fix the maximum size of the message that can be sent  one at a time 
        private const int Threshold = 1025;

        // Declare the TcpClient  variable 
        private readonly TcpClient _clientSocket;

        /// <summary>
        /// This is the constructor of the class which initializes the params
        /// <param name="queue">queue.</param>
        /// <param name="clientSocket">clientSocket.</param>
        /// </summary>
        public ReceiveSocketListener(IQueue queue, TcpClient clientSocket)
        {
            _queue = queue;
            _clientSocket = clientSocket;
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
        /// This forms packet object out of received string
        /// it looks for EOF to know the end of message
        /// </summary>
        /// <returns>Packet </returns>
        private Packet GetPacket(string[] msg)
        {
            Packet packet = new Packet
            {
                ModuleIdentifier = msg[0]
            };

            //get serialized data of packet
            string data = string.Join(":", msg[1..]);

            // search of EOF to get  end of the message
            packet.SerializedData = data[..data.LastIndexOf("EOF", StringComparison.Ordinal)];
            return packet;
        }

        /// <summary>
        /// This method runs on a thread and listen for incoming message
        /// </summary>
        private void Listen()
        {
            //Variable to store the entire message
            string message = "";
            while (_listenRun)
            {
                try
                {
                    //Get NetworkStream to read message
                    NetworkStream networkStream = _clientSocket.GetStream();

                    //read when data is availabe into a buffer
                    while (networkStream.DataAvailable)
                    {
                        byte[] inStream = new byte[Threshold];
                        networkStream.Read(inStream, 0, inStream.Length);
                        string buffer = System.Text.Encoding.ASCII.GetString(inStream);
                        for (int i = 0; i < Threshold; i++)
                        {
                            if (buffer[i]!='\u0000')
                            {
                                message = message + buffer[i];
                                if (message.Contains("EOF"))
                                {
                                    //Calls GetPacket method to form packet object out of received message
                                    Packet packet = GetPacket(message.Split(":"));
                                    //Calls the PushToQueue method to push packet into queue
                                    PushToQueue(packet.SerializedData, packet.ModuleIdentifier);
                                    message = "";
                                }
                            }
                           
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
        }

        /// <summary>
        /// This method closes the listen thread
        /// </summary>
        public void Stop()
        {
            _listenRun = false;
        }

        /// <summary>
        /// This method is for pushing the data into the queue
        /// </summary>
        private void PushToQueue(string data, string moduleIdentifier)
        {
            Packet packet = new Packet {ModuleIdentifier = moduleIdentifier, SerializedData = data};
            Trace.WriteLine("SERVER/CLIENT : " + data);
            _queue.Enqueue(packet);
        }
    }
}