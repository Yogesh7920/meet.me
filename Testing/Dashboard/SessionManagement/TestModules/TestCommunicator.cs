using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class TestCommunicator : ICommunicator
    {
        public TestCommunicator(string address = null)
        {
            userCount = 0;
            if (address == null)
                meetAddress = "192.168.1.1:8080";
            else
                meetAddress = address;
        }
        public void AddClient<T>(string clientId, T socketObject)
        {
            userCount++;
        }

        public void RemoveClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public void Send(string data, string identifier)
        {
            transferredData = data;
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
            
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority)
        {

        }

        public string meetAddress;
        public string transferredData;
        public int userCount;
    }
}