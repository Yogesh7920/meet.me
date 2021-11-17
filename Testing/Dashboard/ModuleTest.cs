using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Sockets;
using Testing.Dashboard;
using Testing.Dashboard.TestModels;

namespace Testing
{
    public class DashboardModuleTests
    {
        [SetUp]
        public void Setup()
        {
            _testCommunicator = new();
            _testCommunicator.ipAddressAndPort = validIP + ":" + validPort;
            clientSessionManagerA = new(_testCommunicator);
            clientSessionManagerB = new(_testCommunicator);
            newUX = new();
            oldUX = new();
            clientSessionManagerB.SubscribeSession(newUX);
            clientSessionManagerA.SubscribeSession(oldUX);
            serverSessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator);
        }

        [Test]
        public void GetPortAndIpAddress_ValidRoomCreation_ReturnsMeetCreds()
        {
            IUXServerSessionManager _sessionManager = serverSessionManager;
            MeetingCredentials returnedMeetCreds;
            MeetingCredentials testMeetCreds;
            _testCommunicator.ipAddressAndPort = Utils.GenerateValidIPAndPort();
            testMeetCreds = Utils.GenerateMeetingCreds(_testCommunicator.ipAddressAndPort);
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(testMeetCreds.ipAddress, returnedMeetCreds.ipAddress);
            Assert.AreEqual(testMeetCreds.port, returnedMeetCreds.port);


        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("abcd.192.1.2:8080")]
        [TestCase("192.1.2:8080")]
        [TestCase("abcdefg")]
        public void GetPortAndIpAddress_InValidRoomCreation_ReturnsNull(string input)
        {
            IUXServerSessionManager _sessionManager = serverSessionManager;
            MeetingCredentials returnedMeetCreds;
            MeetingCredentials testMeetCreds;
            testMeetCreds = null;
            _testCommunicator.ipAddressAndPort = input;
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(testMeetCreds, returnedMeetCreds);
        }

        [Test]
        public void AddClient_ValidClientArrivalClientSide_ReturnsTrue()
        {
            IUXClientSessionManager _sessionManager = clientSessionManagerB;

            // Setting the IP address and Port for fake server
            bool isValid = _sessionManager.AddClient(validIP, int.Parse(validPort), "John");
            bool expectedValue = true;
            Assert.AreEqual(expectedValue, isValid);

            // Testing for invalid IPs and usernames
        }

        [Test]
        [TestCase("192.168.1.2", 8080, "John")]
        [TestCase("192.168.1.1", 8080, "")]
        [TestCase("192.168.1.1", 8081, "John")]
        [TestCase("192.168.1.1", 8080, null)]
        [TestCase("abced", 8080, "John")]
        public void AddClient_InValidClientArrivalClientSide_ReturnsFalse(string ip, int port, string username)
        {
            IUXClientSessionManager _sessionManager = clientSessionManagerB;
            bool expectedValue = false;
            bool isValid = _sessionManager.AddClient(ip, port, username);
            Assert.AreEqual(expectedValue, isValid);
        }

        [Test]
        public void AddClientProcedure_NewClientArrival_BroadcastsUserObjectToAllClients()
        {
            _testCommunicator.sentData = null;
            ClientToServerData clientToServerData = new("addClient", "John");
            string serializedData = _serializer.Serialize(clientToServerData);
            serverSessionManager.OnClientJoined<TcpClient>(null);
            serverSessionManager.OnDataReceived(serializedData);

            ServerToClientData recievedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            UserData user = recievedData.GetUser();
            Assert.NotNull(user);
            Assert.AreEqual("John", user.username);
            Assert.IsNotNull(user.userID);
            Assert.AreEqual("addClient", recievedData.eventType);
        }

        [Test]
        public void AddClientProcedure_NewClientArrival_BroadcastsSessionObjectToAllClients()
        {
            int sampleSize = 10;
            List<UserData> users = Utils.GenerateUserData(sampleSize);
            AddUsersAtServer(users);

            // The last inserted user should get a object that has all the previously added clients
            ServerToClientData recievedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            SessionData sessionData = recievedData.sessionData;
            Assert.NotNull(sessionData);
            CollectionAssert.AreEqual(users, sessionData.users);
            Assert.AreEqual(sampleSize, _testCommunicator.clientCount);
        }

        [Test]
        public void UpdateClientProcedure_ClientArrivalNotification_UpdatesUXAboutChanges()
        {
            int dataSize = 10;

            // SessionData After adding new user
            SessionData sData = Utils.GenerateSampleSessionData(dataSize);

            // The new user is removed as it wont be present before joining
            UserData newUser = sData.users[dataSize - 1];
            sData.users.RemoveAt(dataSize - 1);

            // When the old user joins the first time, it would recieve complete session object
            ServerToClientData serverToClientData = new("addClient", sData, null, sData.users[dataSize - 2]); ; ;
            clientSessionManagerA.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientData));
            oldUX.gotNotified = false;

            // Following are recieved when new user joins for old and new users
            sData.AddUser(newUser);
            ServerToClientData serverToClientDataNew = new("addClient", sData, null, sData.users[dataSize - 1]);
            string serialisedDataNew = _serializer.Serialize(serverToClientDataNew);
            //Console.WriteLine("MT: " + serialisedDataNew);
            clientSessionManagerB.OnDataReceived(serialisedDataNew);
            clientSessionManagerA.OnDataReceived(_serializer.Serialize(serverToClientDataNew));
            //while (newUX.gotNotified == false) ;
            //while (oldUX.gotNotified == false) ;
            Assert.NotNull(oldUX.sessionData);
            Assert.NotNull(newUX.sessionData);
            CollectionAssert.AreEqual(sData.users, oldUX.sessionData.users);
            CollectionAssert.AreEqual(sData.users, newUX.sessionData.users);

        }

        [Test]
        public void RemoveClient_ClientDeparture_SendsServerDepartedUser()
        {
            IUXClientSessionManager _uxSessionManager = clientSessionManagerB;
            string username = "John";
            int userId = 1;
            AddUserClientSide(username, userId);

            _testCommunicator.sentData = null;
            // When client leaves
            _uxSessionManager.RemoveClient();

            while (_testCommunicator.sentData == null) ;
            ClientToServerData deserialisedObject = _serializer.Deserialize<ClientToServerData>(_testCommunicator.sentData);
            Assert.NotNull(deserialisedObject);
            Assert.AreEqual(username, deserialisedObject.username);
            Assert.AreEqual(userId, deserialisedObject.userID);
        }


        [Test]
        public void EndMeet_EndMeeting_SendsEndMeetingEventToServer()
        {
            AddUserClientSide("John", 1);
            clientSessionManagerB.EndMeet();
            string expectedEvent = "endMeet";
            ClientToServerData deserializedObj = _serializer.Deserialize<ClientToServerData>(_testCommunicator.sentData);
            Assert.AreEqual(expectedEvent, deserializedObj.eventType);
        }

        //[TestCase("This is sample summary")]
        //[TestCase(null)]
        //[TestCase("")]
        //[Test]
        //public void GetSummary_GetSummary_ReturnsSummary(string testSummary)
        //{
        //    UserData user = new("John", 1);
        //    // Adding a user at client
        //    AddUserClientSide(user.username, user.userID);
        //    string recievedSummary = null; 
        //    SummaryData summaryData = new(testSummary);
        //    ServerToClientData testData = new("getSummary",null,summaryData, user);
        //    Thread getSummaryThread = new Thread(new ThreadStart(()=> { recievedSummary = clientSessionManagerB.GetSummary(); }));
        //    getSummaryThread.Start();
        //    Thread.Sleep(1000);
        //    clientSessionManagerB.OnDataReceived(_serializer.Serialize(testData));
        //    while (getSummaryThread.IsAlive) ;
        //    Assert.AreEqual(testSummary, recievedSummary);
        //}

        private void AddUserClientSide(string username, int userId, string ip = "192.168.1.1", string port = "8080")
        {
            UserData userData = new(username, userId);
            IUXClientSessionManager _uxSessionManager = clientSessionManagerB;
            INotificationHandler _networkSessionManager = clientSessionManagerB;
            // Creating the user who joined
            ServerToClientData serverToClientData = new("removeClient", null, null, userData);
            string serialisedServerData = _serializer.Serialize(serverToClientData);

            // Adding the client to client first
            _testCommunicator.ipAddressAndPort = ip + ":" + port;
            _uxSessionManager.AddClient(ip, int.Parse(port), "John");
            _networkSessionManager.OnDataReceived(serialisedServerData);
        }

        private void AddUsersAtServer(List<UserData> users)
        {

            for (int i = 0; i < users.Count; i++)
            {
                serverSessionManager.OnClientJoined<TcpClient>(null);
                _testCommunicator.sentData = null;
                ClientToServerData clientToServerData = new("addClient", users[i].username);
                string serializedData = _serializer.Serialize(clientToServerData);
                serverSessionManager.OnDataReceived(serializedData);
            }

        }


        private ClientSessionManager clientSessionManagerA, clientSessionManagerB;
        private ServerSessionManager serverSessionManager;
        private TestUX newUX, oldUX;
        private readonly ISerializer _serializer = new Serializer();
        private TestCommunicator _testCommunicator;
        private readonly string validIP = "192.168.1.1", validPort = "8080";
    }
}