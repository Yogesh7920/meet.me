using NUnit.Framework;
using Networking;
using Testing.Dashboard.TestModels;
using System.Collections.Generic;
using System;
using Testing.Dashboard;
using Dashboard.Server.SessionManagement;
using Dashboard;
using Dashboard.Client.SessionManagement;

namespace Testing
{
    public class DashboardModuleTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RoomCreationTest()
        {
            IUXServerSessionManager _sessionManager = new ServerSessionManager(_testCommunicator);
            MeetingCredentials returnedMeetCreds;
            MeetingCredentials testMeetCreds;
            for (int i = 0; i < validTests; i++)
            {
                // Testing different combinations of valid IPs and Port number
                _testCommunicator.ipAddressAndPort = Utils.GenerateValidIPAndPort();
                testMeetCreds = Utils.GenerateMeetingCreds(_testCommunicator.ipAddressAndPort);
                returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
                Assert.AreEqual(testMeetCreds.port, returnedMeetCreds.port);
                Assert.AreEqual(testMeetCreds.ipAddress, returnedMeetCreds.ipAddress);
            }

            // Testing Invalid Test cases
            List<string> invalidIPs = Utils.GenerateInvalidIPAndPort();
            foreach (string ip in invalidIPs)
            {
                _testCommunicator.ipAddressAndPort = ip;
                returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
                Assert.AreEqual(null, returnedMeetCreds);
            }
        }

        [Test]
        public void ClientArrivalClientSideTest()
        {
            string validIPAddress = "192.168.1.1";
            string port = "8080";
            IUXClientSessionManager sessionManager = SessionManagerFactory.GetClientSessionManager(_testCommunicator);
            
           // Setting the IP address and Port for fake server
            _testCommunicator.ipAddressAndPort = validIPAddress + ":" + port;

            // Testing valid connection
            Assert.AreEqual(true, sessionManager.AddClient(validIPAddress, int.Parse(port), "John"));
            Assert.AreEqual(true, sessionManager.AddClient(validIPAddress, int.Parse(port), "Sam"));

            // Testing for invalid IPs and usernames
            Assert.AreEqual(false, sessionManager.AddClient("192.169.1.2", 2000, "John"));
            Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), null));
            Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), ""));

        }

        [Test]
        public void UserCreationTest()
        {
            ServerSessionManager sessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator);
            _testCommunicator.sentData = null;
            ClientToServerData clientToServerData = new("addClient", "John");
            string serializedData = _serializer.Serialize(clientToServerData);
            sessionManager.OnDataReceived(serializedData);
            
            while (_testCommunicator.sentData == null) ;
            Console.WriteLine(_testCommunicator.sentData);
            ServerToClientData recievedData =  _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            UserData user = recievedData.GetUser();
            Assert.AreEqual("John", user.username);
            Assert.IsNotNull(user.userID);
            Assert.AreEqual("addClient", recievedData.eventType);
            Assert.Pass();
        }


        [Test]
        public void sessionObjectTest()
        {
            List<UserData> users = Utils.GenerateUserData();

            for(int i=0; i<3; i++)
            {
                ServerSessionManager sessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator);
                _testCommunicator.sentData = null;
                ClientToServerData clientToServerData = new("addClient", users[i].username);
                string serializedData = _serializer.Serialize(clientToServerData);
                sessionManager.OnDataReceived(serializedData);
            }

            while (_testCommunicator.sentData == null) ;
            Console.WriteLine(_testCommunicator.sentData);
            ServerToClientData recievedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            SessionData sessionData = recievedData.sessionData;
            CollectionAssert.AreEqual(users, sessionData.users);
        }

        private ISerializer _serializer = new Serializer();
        private readonly int validTests = 10;
        private readonly TestCommunicator _testCommunicator = new TestCommunicator();
    }
}