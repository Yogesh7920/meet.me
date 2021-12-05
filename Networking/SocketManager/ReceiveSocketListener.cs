/// <author>Tausif Iqbal</author>
/// <created>13/10/2021</created>
/// <summary>
/// This file contains the class definition of ReceiveSocketListener.
/// </summary>

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
        private const int Threshold = 1024 * 1024;

        // Declare the TcpClient  variable 
        private readonly Socket _clientSocket;
        private byte[] buffer = new byte[Threshold];
        private readonly StringBuilder _message = new();

        // Declare the queue variable which is used to dequeue the required the packet 
        private readonly IQueue _queue;

        // Declare the thread variable of ReceiveSocketListener 
        private Thread _listen;

        // Declare variable that dictates the start and stop of the thread _listen
        private volatile bool _listenRun;

        /// <summary>
        ///     This is the constructor of the class which initializes the params
        ///     <param name="queue">An object of type IQueue that is used by the communicator</param>
        ///     <param name="tcpClient">The socket object of the connection</param>
        /// </summary>
        public ReceiveSocketListener(IQueue queue, TcpClient tcpClient)
        {
            _queue = queue;
            tcpClient.GetStream();
            _clientSocket = tcpClient.Client;
        }

        private string ProcessPackets(string message)
        {
            while (true)
            {
                var isMessage = false;
                var packetString = "";
                if (message == "") break;
                // get the index of the next two flags
                var flagIndex = message.IndexOf(Utils.Flag, StringComparison.Ordinal);
                var nextFlagIndex = message.IndexOf(Utils.Flag, flagIndex + 5, StringComparison.Ordinal);
                while (!isMessage)
                {
                    if (nextFlagIndex == -1)
                        break;
                    if (message[(nextFlagIndex - 5)..nextFlagIndex] == Utils.Esc)
                    {
                        // if the message is of the form [ESC][FLAG], ignore and continue
                        nextFlagIndex = message.IndexOf(Utils.Flag, nextFlagIndex + 6,
                            StringComparison.Ordinal);
                        continue;
                    }

                    packetString = message[(flagIndex + 6)..nextFlagIndex];
                    message = message[(nextFlagIndex + 6)..];
                    isMessage = true;
                }

                if (isMessage)
                {
                    packetString = packetString.Replace($"{Utils.Esc}{Utils.Esc}", $"{Utils.Esc}");
                    packetString = packetString.Replace($"{Utils.Esc}{Utils.Flag}", $"{Utils.Flag}");
                    var packet = GetPacket(packetString.Split(":"));
                    PushToQueue(packet.SerializedData, packet.ModuleIdentifier);
                    continue;
                }

                break;
            }

            return message;
        }
        
        private void ReceiveCallback( IAsyncResult ar ) {  
            try {
                if (!_listenRun)
                {
                    return;
                }
                // Read data from the remote device.  
                int bytesRead = _clientSocket.EndReceive(ar);  
                if (bytesRead > 0) {  
                    // There might be more data, so store the data received so far.  
                    _message.Append(Encoding.ASCII.GetString(buffer,0,bytesRead));
                    string msg = _message.ToString();
                    msg = ProcessPackets(msg);
                    _message.Clear();
                    _message.Append(msg);
                }
                _clientSocket.BeginReceive(buffer,0, Threshold,0, ReceiveCallback, null);
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }

        /// <summary>
        ///     This method is for starting the thread
        /// </summary>
        /// <returns> Void  </returns>
        public void Start()
        {
            _listen = new Thread(() => _clientSocket.BeginReceive(buffer,0, Threshold,0, ReceiveCallback, null));
            _listenRun = true;
            _listen.Start();
            Trace.WriteLine("[Networking] ReceiveSocketListener thread started.");
        }

        /// <summary>
        ///     This forms packet object out of received string
        ///     it looks for EOF to know the end of message
        /// </summary>
        /// <param name="msg"> string containing data</param>
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
            packet.SerializedData = data;
            return packet;
        }

        /// <summary>
        ///     This method closes the listen thread
        /// </summary>
        /// <returns> Void  </returns>
        public void Stop()
        {
            _listenRun = false;
            Trace.WriteLine("[Networking] Stopped ReceiveSocketListener thread.");
        }

        /// <summary>
        ///     This method is for pushing the data into the queue
        /// </summary>
        /// <param name="data"> packet data</param>
        /// <param name="moduleIdentifier"> module ID </param>
        /// <returns> Void  </returns>
        private void PushToQueue(string data, string moduleIdentifier)
        {
            var packet = new Packet {ModuleIdentifier = moduleIdentifier, SerializedData = data};
            Trace.WriteLine($"[Networking] Received data from {moduleIdentifier}");
            _queue.Enqueue(packet);
        }
    }
}