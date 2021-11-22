using Networking;
using System;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class TestCommunicator : ICommunicator
    {
        public TestCommunicator(string address = null)
        {
            if (address == null)
                meetAddress = "192.168.1.1:8080";
            else
                meetAddress = address;
        }
        public void AddClient<T>(string clientId, T socketObject)
        {
            throw new NotImplementedException();
        }

        public void RemoveClient(string clientId)
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

        public string Start(string serverIp = null, string serverPort = null)
        {
            if (serverIp == null && serverPort == null)
                return meetAddress;

            if (meetAddress == (serverIp + ":" + serverPort))
                return "1";
            return "0";
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority)
        {

        }

        public string meetAddress;
    }
}