using System;

namespace Networking
{
    public static class CommunicationFactory
    {
        // Communicator Instance
        private static readonly Lazy<ICommunicator> _clientCommunicator = new Lazy<ICommunicator>(() => new ClientCommunicator());
        private static readonly Lazy<ICommunicator> _serverCommunicator = new Lazy<ICommunicator>(() => new ServerCommunicator());

        /// <summary>
        /// Returns the Communicator instance that is running.
        /// </summary>
        /// <returns>ICommunicator.</returns>
        public static ICommunicator GetCommunicator(bool isClient = true)
        {
            if (isClient)
                return _clientCommunicator.Value;
            return _serverCommunicator.Value;
        }
    }
}