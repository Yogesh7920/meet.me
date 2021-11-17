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
            _testCommunicator = new();
            oldCSessionManager = new(_testCommunicator);
            newCSessionManager = new(_testCommunicator);
            newUX = new();
            oldUX = new();
            newCSessionManager.SubscribeSession(newUX);
            oldCSessionManager.SubscribeSession(oldUX);
            serverSessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator);
        }

        [Test]
        public void RoomCreationTest()
        {
            IUXServerSessionManager _sessionManager = serverSessionManager;
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
            IUXClientSessionManager _sessionManager = newCSessionManager;
            
           // Setting the IP address and Port for fake server
            _testCommunicator.ipAddressAndPort = validIPAddress + ":" + port;

            // Testing valid connection
            Assert.AreEqual(true, _sessionManager.AddClient(validIPAddress, int.Parse(port), "John"));
            Assert.AreEqual(true, _sessionManager.AddClient(validIPAddress, int.Parse(port), "Sam"));

            // Testing for invalid IPs and usernames
            Assert.AreEqual(false, _sessionManager.AddClient("192.169.1.2", 2000, "John"));
            // Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), null));
            // Assert.AreEqual(false, sessionManager.AddClient(validIPAddress, int.Parse(port), ""));

        }

        [Test]
        public void UserCreationTest()
        {
            _testCommunicator.sentData = null;
            ClientToServerData clientToServerData = new("addClient", "John");
            string serializedData = _serializer.Serialize(clientToServerData);
            serverSessionManager.OnDataReceived(serializedData);
            
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

            for(int i=0; i<users.Count; i++)
            {
                ServerSessionManager sessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator);
                _testCommunicator.sentData = null;
                ClientToServerData clientToServerData = new("addClient", users[i].username);
                string serializedData = _serializer.Serialize(clientToServerData);
                sessionManager.OnDataReceived(serializedData);
            }

            while (_testCommunicator.sentData == null) ;

            // The last inserted user should get a object that has all the previously added clients
            Console.WriteLine(_testCommunicator.sentData);
            ServerToClientData recievedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            SessionData sessionData = (SessionData)recievedData.GetObject();
            CollectionAssert.AreEqual(users, sessionData.users);
        }

        [Test]
        public void ClientArrivalNotificationTest()
        {
            int dataSize = 10;
            
            // SessionData After adding new user
            SessionData sData = Utils.GenerateSampleSessionData(dataSize, "addClient");
            
            // The new user is removed as it wont be present before joining
            UserData newUser = sData.users[dataSize - 1];
            sData.users.RemoveAt(dataSize - 1);

            // When the old user joins the first time, it would recieve complete session object
            ServerToClientData serverToClientData = new("addClient", sData, sData.users[dataSize-2]);
            oldCSessionManager.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientData));
            oldUX.gotNotified = false;

            // Following are recieved when new user joins for old and new users
            sData.AddUser(newUser);
            ServerToClientData serverToClientDataNew = new("addClient", sData, sData.users[dataSize-1]);
            string serialisedDataNew =  _serializer.Serialize<ServerToClientData>(serverToClientDataNew);
            //Console.WriteLine("MT: " + serialisedDataNew);
            newCSessionManager.OnDataReceived(serialisedDataNew);
            oldCSessionManager.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientDataNew));
            while (newUX.gotNotified == false) ;
            while (oldUX.gotNotified == false) ;

            CollectionAssert.AreEqual(sData.users, oldUX.sessionData.users);
            CollectionAssert.AreEqual(sData.users, newUX.sessionData.users);

        }

        [Test]
        public void ClientDepartureClientSide()
        {
            IUXClientSessionManager _uxSessionManager = newCSessionManager;
            INotificationHandler _networkSessionManager = newCSessionManager;
            string username = "John";
            int userId = 1;
            AddUserClientSide(username, userId);

            _testCommunicator.sentData = null;
            // When client leaves
            _uxSessionManager.RemoveClient();

            while (_testCommunicator.sentData == null) ;
            ClientToServerData deserialisedObject = _serializer.Deserialize<ClientToServerData>(_testCommunicator.sentData); 
            Assert.AreEqual(username, deserialisedObject.username);
            Assert.AreEqual(userId, deserialisedObject.userID);
        }

        private void AddUserClientSide(string username, int userId, string ip="192.168.1.1", string port = "8080")
        {
            UserData userData = new(username, userId);
            IUXClientSessionManager _uxSessionManager = newCSessionManager;
            INotificationHandler _networkSessionManager = newCSessionManager;
            // Creating the user who joined
            ServerToClientData serverToClientData = new("removeClient", null, userData);
            string serialisedServerData = _serializer.Serialize(serverToClientData);

            // Adding the client to client first
            _testCommunicator.ipAddressAndPort = ip + ":" + port;
            _uxSessionManager.AddClient(ip, int.Parse(port), "John");
            _networkSessionManager.OnDataReceived(serialisedServerData);
        }

        private ClientSessionManager oldCSessionManager, newCSessionManager;
        private ServerSessionManager serverSessionManager;
        private TestUX newUX, oldUX;
        private readonly ISerializer _serializer = new Serializer();
        private readonly int validTests = 10;
        private TestCommunicator _testCommunicator;
    }
}