namespace Networking
{
    public class SendSocketListener{

        // It contains the packets which needs to be sent over the network
        private IQueue _queue;

        /// <summary>
        /// This method is the constructor of the class which initializes the params
        /// <param name="queue">queue</param>
        /// </summary>
        public SendSocketListener(IQueue queue){
            this._queue = queue;
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
    }
}