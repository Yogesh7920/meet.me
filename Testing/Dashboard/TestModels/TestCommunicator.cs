using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace Testing.Dashboard.TestModels
{
    public class TestCommunicator : ICommunicator 
    {
        public TestCommunicator()
        {

        }

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
            sentData = data;
        }

        public void Send(string data, string identifier, string destination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// start function for testing room creation
        /// </summary>
        /// <returns> string port and IP needed for testing </returns>
        public string Start(string serverIP = null, string serverPort = null)
        {
            if(serverIP == null && serverPort == null)
                return ipAddressAndPort;
            if (serverIP + ":" + serverPort == ipAddressAndPort)
                return "1";
            return "0";
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority=1)
        {
            
        }

        public string ipAddressAndPort;
        public string sentData;

    }
}
