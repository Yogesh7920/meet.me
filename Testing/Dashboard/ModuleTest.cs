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
            // Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), null));
            // Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), ""));

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
        public void SessionObjectTest()
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
            SessionData sessionData = (SessionData)recievedData.GetObject();
            CollectionAssert.AreEqual(users, sessionData.users);
        }

        [Test]
        public void ClientArrivalNotificationTest()
        {
            Console.WriteLine("check Point");
            int dataSize = 10;
            // New Arrived user
            ClientSessionManager cSessionManagerNew = new(_testCommunicator);
            TestUX testUXNew = new();
            cSessionManagerNew.SubscribeSession(testUXNew);

            // Old users
            ClientSessionManager cSessionManagerOld = new(_testCommunicator);
            TestUX testUXOld = new();
            cSessionManagerOld.SubscribeSession(testUXOld);
            
            // SessionData After adding new user
            SessionData sData = Utils.GenerateSampleSessionData(dataSize, "addClient");
            
            // The new user is removed as it wont be present before joining
            UserData newUser = sData.users[dataSize - 1];
            sData.users.RemoveAt(dataSize - 1);

            // When the old user joins the first time, it would recieve complete session object
            ServerToClientData serverToClientData = new("addClient", sData, sData.users[dataSize-2]);
            cSessionManagerOld.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientData));
            testUXOld.gotNotified = false;

            // Following are recieved when new user joins for old and new users
            sData.AddUser(newUser);
            ServerToClientData serverToClientDataNew = new("addClient", sData, sData.users[dataSize-1]);
            string serialisedDataNew =  _serializer.Serialize<ServerToClientData>(serverToClientDataNew);
            //Console.WriteLine("MT: " + serialisedDataNew);
            cSessionManagerNew.OnDataReceived(serialisedDataNew);
            cSessionManagerOld.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientDataNew));
            while (testUXNew.gotNotified == false) ;
            while (testUXOld.gotNotified == false) ;

            CollectionAssert.AreEqual(sData.users, testUXOld.sessionData.users);
            CollectionAssert.AreEqual(sData.users, testUXNew.sessionData.users);

        }

        private ISerializer _serializer = new Serializer();
        private readonly int validTests = 10;
        private readonly TestCommunicator _testCommunicator = new();
    }
}