namespace Networking
{
    public static class CommunicationFactory
    {
        private static ICommunicator _communicator;
        static CommunicationFactory()
        {
            _communicator = new ClientCommunicator();
        }
        /// <summary>
        /// Returns the Communicator instance that is running.
        /// </summary>
        /// <returns>ICommunicator.</returns>
        public static ICommunicator GetCommunicator()
        {
            return _communicator;
        }
    }
}
