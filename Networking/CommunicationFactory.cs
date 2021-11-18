using System;

namespace Networking
{
    public static class CommunicationFactory
    {
        // Communicator Instance
        private static readonly Lazy<ICommunicator> SClientCommunicator = new(() => new ClientCommunicator());
        private static readonly Lazy<ICommunicator> SServerCommunicator = new(() => new ServerCommunicator());

        /// <summary>
        ///     Returns the Communicator instance that is running.
        ///     In test mode, It always returns a new instance.
        /// </summary>
        /// <returns>ICommunicator.</returns>
        public static ICommunicator GetCommunicator(bool isClient = true, bool isTesting = false)
        {
            if (isClient)
                return isTesting ? new ClientCommunicator() : SClientCommunicator.Value;
            return isTesting ? new ServerCommunicator() : SServerCommunicator.Value;
        }
    }
}