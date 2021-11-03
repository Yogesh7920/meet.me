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
        public static ICommunicator GetCommunicator(bool isClient=true)
        {
            if (isClient)
            {
                if (_clientCommunicator == null)
                    _clientCommunicator = new ClientCommunicator();
                return _clientCommunicator;   
            }
            if (_serverCommunicator == null)
                _serverCommunicator = new ServerCommunicator();
            return _serverCommunicator;
        }
    }
}
