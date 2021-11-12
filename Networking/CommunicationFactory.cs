namespace Networking
{
    public static class CommunicationFactory
    {
        private static ICommunicator _clientCommunicator;
        private static ICommunicator _serverCommunicator;
        /// <summary>
        /// Returns the Communicator instance that is running.
        /// </summary>
        /// <returns>ICommunicator.</returns>
        /// TODO Make this thread-safe: https://www.c-sharpcorner.com/UploadFile/8911c4/singleton-design-pattern-in-C-Sharp/
        public static ICommunicator GetCommunicator(bool isClient=true , bool isTesting=false)
        {
            if (isClient)
            {
                if (isTesting)
                {
                    _clientCommunicator = new ClientCommunicator(true);
                    return _clientCommunicator;  
                }

                if (_clientCommunicator == null)
                {
                    _clientCommunicator = new ClientCommunicator();
                }
                    
                return _clientCommunicator;   
                
            }
            if (isTesting)
            {
                _serverCommunicator = new ServerCommunicator(true);
                return _serverCommunicator;  
            }

            return _serverCommunicator ??= new ServerCommunicator();
        }
    }
}