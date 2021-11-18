using System;
using Networking;

namespace Testing.Dashboard.TestModels
{
    public class TestCommunicator : ICommunicator
    {
        public string ipAddressAndPort;

        public void AddClient<T>(string clientID, T socketObject)
        {
            throw new NotImplementedException();
        }

        public void RemoveClient(string clientID)
        {
            throw new NotImplementedException();
        }

        public void Send(string data, string identifier)
        {
            throw new NotImplementedException();
        }

        public void Send(string data, string identifier, string destination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     start function for testing room creation
        /// </summary>
        /// <returns> string port and IP needed for testing </returns>
        public string Start(string serverIP = null, string serverPort = null)
        {
            return ipAddressAndPort;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority = 1)
        {
        }
    }
}