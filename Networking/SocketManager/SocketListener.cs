namespace Networking
{
    public class SocketListener{

        // queue containing messages
        private IQueue _queue;

        // ipAddress of the destination
        private string _ipAddress;

        // Port of the destination
        private string _port;

        //socket which keeps listening for the client requests
        private Socket _socket;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue.</param>
        /// <param name="ipAddress">ipAddress.</param>
        /// <param name="port">port.</param>
        /// </summary>
        public SocketListener(IQueue queue,string ipAddress ,string port){
            this._queue=queue;
            this._ipAddress=ipAddress;
            this._port=port;

        }

        /// <summary>
        /// This method is for starting the thread
        /// </summary>
        public void Start(){

        }

        /// <summary>
        /// This method is for stopping the thread
        /// </summary>
        public void Stop(){

        }

        /// <summary>
        /// This method is for pushing the data into the queue
        /// </summary>
        private void PushToQueue(string data, string moduleIdentifier){

        }
    }
}