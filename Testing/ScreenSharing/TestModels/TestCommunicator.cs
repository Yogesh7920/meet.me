using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace Testing.ScreenSharing.TestModels
{
    public class TestCommunicator : ICommunicator
    {
        public string ipAddressAndPort;
        public bool isCommunicatorStopped;
        public int clientCount;
        public string sentData;

        public TestCommunicator()
        {
            sentData = null;
            isCommunicatorStopped = false;
        }
        public string Start(string serverIP = null, string serverPort = null)
        {
            if (serverIP == null && serverPort == null)
                return ipAddressAndPort;
            if (serverIP + ":" + serverPort == ipAddressAndPort)
                return "1";
            return "0";
        }

        public void Stop()
        {
            isCommunicatorStopped = true;
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority = 1)
        {

        }

        public void AddClient<T>(string clientID, T socketObject)
        {
            clientCount++;
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
    }
}
