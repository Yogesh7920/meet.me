using System.Net.Sockets;

/// <summary>
/// This file contains the implementation of socketListener
/// socketListener will continously listen for message and
/// after getting the message it will push into queue 
/// </summary>
/// <author>Tausif Iqbal </author>
namespace Networking
{
    public class SocketListener{
        Thread listen;
        // define the threashold size
        const int threshold = 1025;

        // queue containing messages
        private IQueue _queue;

        //socket which keeps listening for the client requests
        private TcpClient _clientSocket;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue.</param>
        /// <param name="clientSocket">clientSocket.</param>
        /// </summary>
        public SocketListener(IQueue queue,TcpClient clientSocket){
            this._queue=queue;
            this._clientSocket=clientSocket;
            listen = new Thread(Start);
            listen.Start();
        }

        /// <summary>
        /// This method is for starting the thread
        /// </summary>
        public void Start(){
            while ((true))
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    String message = "";
                    while (true)
                    {
                        byte[] inStream = new byte[threshold];
                        networkStream.Read(inStream, 0, inStream.Length);
                        message += System.Text.Encoding.ASCII.GetString(inStream);
                        if (message.Contains("EOF"))
                        {
                            break;
                        }
                    }
                    //push to Queue
              
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> listen method " + ex.ToString());
                    break;
                }
            }
        }

        /// <summary>
        /// This method closes the listen thread
        /// </summary>
        public void Stop(){
            listen.Abort();
        }

        /// <summary>
        /// This method is for pushing the data into the queue
        /// </summary>
        private void PushToQueue(string data, string moduleIdentifier){
            throw new NotImplementedException();
        }
    }
}