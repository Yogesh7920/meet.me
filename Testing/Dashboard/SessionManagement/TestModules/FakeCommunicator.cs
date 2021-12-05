/// <author> Rajeev Goyal </author>
/// <created> 18/112021 </created>
/// <summary>
/// This file contains a fake communicator which notifies the server and client session manager
/// about any data receives and client joining.
/// </summary>

using System;
using Networking;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeCommunicator : ICommunicator
    {
        public string meetAddress;
        public string transferredData;
        public int userCount;

        public FakeCommunicator(string address = null)
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

            if (meetAddress == serverIp + ":" + serverPort)
                return "1";
            return "0";
        }

        public void Stop()
        {
        }

        public void Subscribe(string identifier, INotificationHandler handler, int priority)
        {
        }
    }
}