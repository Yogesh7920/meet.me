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
    public class RecieveSocketListener{
        private Thread _listen;
        private volatile bool _listenRun =false ;
        // define the threashold size
        const int _threshold = 1025;

        // queue containing messages
        private IQueue _queue;

        //socket which keeps listening for the client requests
        private TcpClient _clientSocket;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue.</param>
        /// <param name="clientSocket">clientSocket.</param>
        /// </summary>
        public RecieveSocketListener(IQueue queue,TcpClient clientSocket){
            this._queue=queue;
            this._clientSocket=clientSocket;
        }

        /// <summary>
        /// This method is for starting the thread
        /// </summary>
        public void Start(){
            _listen = new Thread(() => Listen());
            _listenRun = true;
            _listen.Start();
        }

        /// <summary>
        /// This form packet object out of string
        /// </summary>
        ///  /// <returns>Packet </returns>
        private Packet GetPacket(String msg)
        {
            Packet packet= new Packet();
            string[] s = msg.Split(":");
            packet.ModuleIdentifier = s[0];
            string data = s[1];
            string serializedData = "";
            // have to make it more readable 
            for(int i = 0; i < data.Length - 3; i++)
            {
                if(data[i]=='E' && data[i+1]=='O'&& data[i + 2] == 'F')
                {
                    break;
                }
                serializedData += data[i];
            }
            
            packet.SerializedData = serializedData;
            return packet;
        }

        /// <summary>
        /// This method runs on a thread and listen for incoming message
        /// </summary>
        private void Listen()
        {
            while (_listenRun)
            {
                try
                {
                    NetworkStream networkStream = _clientSocket.GetStream();
                    String message = "";
                    while (_listenRun)
                    {
                        byte[] inStream = new byte[_threshold];
                        networkStream.Read(inStream, 0, inStream.Length);
                        message += System.Text.Encoding.ASCII.GetString(inStream);
                        if (message.Contains("EOF"))
                        {
                            // need to do resizing here else it will print 1KB length of string
                            Trace.WriteLine("message recieved from client " + message);
                            break;
                        }
                    }
                    
                    Packet packet = GetPacket(message);
                    _queue.Enqueue(packet);
                    Trace.WriteLine("message reiceved and equeued into queue");

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(" >> listen method " + ex.ToString());
                    break;
                }
            }
        }

        /// <summary>
        /// This method closes the listen thread
        /// </summary>
        public void Stop(){
            _listenRun = false;
        }

        /// <summary>
        /// This method is for pushing the data into the queue
        /// </summary>
        private void PushToQueue(string data, string moduleIdentifier){
            throw new NotImplementedException();
        }
    }
}