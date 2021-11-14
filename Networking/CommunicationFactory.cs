using System;

namespace Networking
{
    public static class CommunicationFactory
    {
        // Communicator Instance
        private static readonly Lazy<ICommunicator> _clientCommunicator = new(() => new ClientCommunicator());
        private static readonly Lazy<ICommunicator> _serverCommunicator = new(() => new ServerCommunicator());
        /// <summary>
        /// Returns the Communicator instance that is running.
        /// In test mode, It always returns a new instance.
        /// </summary>
        /// <returns>ICommunicator.</returns>
        public static ICommunicator GetCommunicator(bool isClient = true, bool isTesting = false)
        {
            if (isClient)
                return isTesting ? new ClientCommunicator() : _clientCommunicator.Value;
            return isTesting ? new ServerCommunicator() : _serverCommunicator.Value;
        }
    }
}