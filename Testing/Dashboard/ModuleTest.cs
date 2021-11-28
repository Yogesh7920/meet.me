/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the modular tests
///		for dashboard module
/// </summary>

using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.Persistence;
using Dashboard.Server.SessionManagement;
using Dashboard.Server.Telemetry;
using Networking;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Testing.Dashboard.TestModels;

namespace Testing.Dashboard
{
    public class ModuleTests
    {
        [SetUp]
        public void Setup()
        {
            _testContentServer = new();
            _testCommunicator = new();
            _testCommunicator.ipAddressAndPort = validIP + ":" + validPort;
            _testWhiteBoard = new TestWhiteBoard();
            clientSessionManagerA = SessionManagerFactory.GetClientSessionManager(_testCommunicator, _testWhiteBoard);
            clientSessionManagerB = SessionManagerFactory.GetClientSessionManager(_testCommunicator, _testWhiteBoard);
            newUX = new(clientSessionManagerB);
            oldUX = new(clientSessionManagerA);
            clientSessionManagerB.SubscribeSession(newUX);
            clientSessionManagerA.SubscribeSession(oldUX);
            serverSessionManager = SessionManagerFactory.GetServerSessionManager(_testCommunicator, _testContentServer);
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
        public void AddClientProcedure_NewClientArrivalServerSide_BroadcastsUserObjectToAllClients()
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
        public void AddClientProcedure_NewClientArrivalServerSide_BroadcastsSessionObjectToAllClientsAndUpdatesNetworkModule()
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
        public void UpdateClientProcedure_ClientArrivalNotificationClientSide_UpdatesUXAboutChanges()
        {
            int dataSize = 10;

            // SessionData After adding new user
            SessionData sData = Utils.GenerateSampleSessionData(dataSize);

            // The new user is removed as it wont be present before joining
            UserData newUser = sData.users[dataSize - 1];
            sData.users.RemoveAt(dataSize - 1);

            // When the old user joins the first time, it would recieve complete session object
            ServerToClientData serverToClientData = new("addClient", sData, null, null, sData.users[dataSize - 2]); ; ;
            clientSessionManagerA.OnDataReceived(_serializer.Serialize<ServerToClientData>(serverToClientData));
            oldUX.gotNotified = false;

            // Following are recieved when new user joins for old and new users
            sData.AddUser(newUser);
            ServerToClientData serverToClientDataNew = new("addClient", sData, null, null, sData.users[dataSize - 1]);
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
            Assert.AreEqual(newUser.userID.ToString(), _testWhiteBoard.userId);

        }

        [Test]
        public void RemoveClient_ClientDepartureClientSide_SendsServerDepartedUser()
        {
            IUXClientSessionManager _uxSessionManager = clientSessionManagerB;
            string username = "John";
            int userId = 1;
            AddUserClientSide(username, userId);

            _testCommunicator.sentData = null;
            // When client leaves
            _uxSessionManager.RemoveClient();

            ClientToServerData deserialisedObject = _serializer.Deserialize<ClientToServerData>(_testCommunicator.sentData);
            Assert.NotNull(deserialisedObject);
            Assert.AreEqual("removeClient", deserialisedObject.eventType);
            Assert.AreEqual(username, deserialisedObject.username);
            Assert.AreEqual(userId, deserialisedObject.userID);
        }

        [Test]
        //[TestCase(1, 1)]
        [TestCase(10, 5)]
        [TestCase(2, 1)]
        public void RemoveClientProcedure_ClientDepartsServerSide_ReturnsModifiedSessionObject(int sampleSize, int userIndex)
        {
            INotificationHandler networkServerSessionManager = serverSessionManager;

            // Adding sampleSize users at server
            List<UserData> expectedUsers = Utils.GenerateUserData(sampleSize);
            AddUsersAtServer(expectedUsers);
            UserData departedUser = expectedUsers[userIndex - 1];
            expectedUsers.RemoveAt(userIndex - 1);
            string expectedEventType = "removeClient";

            // Data Sample what will be sent from Client to Server
            ClientToServerData leavingUser = new("removeClient", departedUser.username, departedUser.userID);
            string serializedData = _serializer.Serialize(leavingUser);

            // Triggering Remove Client Procedure on Server Dashboard
            networkServerSessionManager.OnDataReceived(serializedData);
            ServerToClientData recievedServerData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            SessionData recievedSessionData = recievedServerData.sessionData;

            CollectionAssert.AreEqual(expectedUsers, recievedSessionData.users);
            CollectionAssert.AreEqual(expectedEventType, recievedServerData.eventType);
        }

        [Test]
        public void EndMeet_EndMeetingClientSide_SendsEndMeetingEventToServer()
        {
            AddUserClientSide("John", 1);
            clientSessionManagerB.EndMeet();
            string expectedEvent = "endMeet";
            ClientToServerData deserializedObj = _serializer.Deserialize<ClientToServerData>(_testCommunicator.sentData);
            Assert.AreEqual(expectedEvent, deserializedObj.eventType);
        }

        [Test]
        public void EndMeet_RecievedEndMeetingEventClientSide_SendsEndMeetingEventToUX()
        {
            ServerToClientData endMeetingMessage = new("endMeet", null, null, null, null);
            clientSessionManagerB.OnDataReceived(_serializer.Serialize<ServerToClientData>(endMeetingMessage));
            Assert.IsTrue(newUX.meetingEndEvent);
        }

        [Test]
        public void EndMeetingProcedure_MeetingEnds_SaveSummaryOfSession()
        {
            _testContentServer.chats = Utils.GetSampleChatContext();
            ClientToServerData sampleClientRequest = new("endMeet", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            string path = "../../../Persistence/PersistenceDownloads/SummaryDownloads/";
            string actualSavedSummary = File.ReadAllText(Path.Combine(path, PersistenceFactory.lastSaveResponse.FileName));
            Assert.IsNotEmpty(actualSavedSummary);
        }


        [Test]
        public void EndMeetingProcedure_MeetingEnds_SaveServerAnalytics()
        {
            int expectedUsers = 10;
            _testContentServer.chats = Utils.GetSampleChatContext();
            List<UserData> users = Utils.GenerateUserData(expectedUsers);
            AddUsersAtServer(users);
            _testContentServer.chats = Utils.GetSampleChatContextForUsers(users);
            ClientToServerData sampleClientRequest = new("endMeet", users[0].username, users[0].userID);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            string serverDataPath = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";
            Assert.IsTrue(File.Exists(Path.Combine(serverDataPath, "GlobalServerData.xml")));
            Directory.Delete("../../../Persistence", true);
        }

        [Test]
        public void EndMeetingProcedure_MeetingEndsWhenOnlyOneUser_SaveServerAnalytics()
        {
            int expectedUsers = 1;
            _testContentServer.chats = Utils.GetSampleChatContext();
            List<UserData> users = Utils.GenerateUserData(expectedUsers);
            AddUsersAtServer(users);
            ClientToServerData sampleClientRequest = new("endMeet", users[0].username, users[0].userID);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            string serverDataPath = "../../../Persistence/PersistenceDownloads/TelemetryDownloads/ServerData";
            Assert.IsTrue(File.Exists(Path.Combine(serverDataPath, "GlobalServerData.xml")));
            Directory.Delete("../../../Persistence", true);
        }


        [Test]
        public void EndMeetingProcedure_MeetingEnds_SendEndMeetingEventToClients()
        {
            _testContentServer.chats = Utils.GetSampleChatContext();
            ClientToServerData sampleClientRequest = new("endMeet", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            ServerToClientData deserialisedReceivedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            string actualEvent = deserialisedReceivedData.eventType;
            Assert.AreEqual("endMeet", actualEvent);
        }

        [TestCase("This is sample summary")]
        [TestCase("")]
        [Test]
        public void GetSummary_GetSummaryClientSide_UpdatesUXWithModifiedSummary(string testSummary)
        {
            UserData user = new("John", 1);
            // Adding a user at client
            AddUserClientSide(user.username, user.userID);
            string recievedSummary = null;
            SummaryData summaryData = new(testSummary);
            ServerToClientData testData = new("getSummary", null, summaryData, null, user);
            clientSessionManagerB.GetSummary();
            clientSessionManagerB.OnDataReceived(_serializer.Serialize(testData));
            recievedSummary = newUX.summary;
            Assert.AreEqual(testSummary, recievedSummary);
        }


        [Test]
        public void GetSummaryProcedure_GetSummarryServerSideWhenChatContextNull_ReturnsEmptyString()
        {
            _testContentServer.chats = null;
            ClientToServerData sampleClientRequest = new ClientToServerData("getSummary", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            ServerToClientData deserialisedReceivedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            SummaryData actualSummaryDataObj = deserialisedReceivedData.summaryData;
            Assert.IsNull(actualSummaryDataObj);
        }

        [Test]
        public void GetSummaryProcedure_GetSummarryServerSideWhenNoChats_ReturnsEmptyString()
        {
            _testContentServer.chats = new();
            ClientToServerData sampleClientRequest = new ClientToServerData("getSummary", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            ServerToClientData deserialisedReceivedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            string actualSummary = deserialisedReceivedData.summaryData.summary;
            Assert.IsEmpty(actualSummary);
        }

        [Test]
        public void GetSummaryProcedure_GetSummarryServerSideSmallSampleChats_ReturnsSummaryString()
        {
            _testContentServer.chats = Utils.GetSampleChatContext();
            ClientToServerData sampleClientRequest = new("getSummary", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            ServerToClientData deserialisedReceivedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            string actualSummary = deserialisedReceivedData.summaryData.summary;
            Assert.IsNotEmpty(actualSummary);
        }

        [Test]
        public void GetSummaryProcedure_GetSummarryServerSideLargeSampleChats_ReturnsSummaryString()
        {
            _testContentServer.chats = Utils.GetSampleChatContext(1000);
            ClientToServerData sampleClientRequest = new("getSummary", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            ServerToClientData deserialisedReceivedData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            string actualSummary = deserialisedReceivedData.summaryData.summary;
            Assert.IsNotEmpty(actualSummary);
        }

        [Test]
        public void GetAnalyticsProcedure_GetAnalyticsServerSide_SendsSessionAnalyticsObjectOnNetwork()
        {
            int expectedUsers = 10;
            List<UserData> users = Utils.GenerateUserData(expectedUsers);
            AddUsersAtServer(users);
            _testContentServer.chats = Utils.GetSampleChatContextForUsers(users);
            ClientToServerData sampleClientRequest = new("getAnalytics", "John", 1);
            serverSessionManager.OnDataReceived(_serializer.Serialize(sampleClientRequest));
            SessionAnalytics actualAnalytics = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData).sessionAnalytics;
            Assert.NotNull(actualAnalytics.chatCountForEachUser);
            Assert.NotNull(actualAnalytics.userCountAtAnyTime);
            Assert.NotNull(actualAnalytics.insincereMembers);
            Assert.AreEqual(actualAnalytics.userCountAtAnyTime.Count, expectedUsers);
        }

        [Test]
        public void GetAnalytics_TelemetryAnalyticsRetrievalClientSide_UpdatesUXWithTelemetryAnalytics()
        {
            UserData user = new("John", 1);
            SessionAnalytics expectedData = Utils.GenerateSessionAnalyticsData();
            // Adding a user at client
            ServerToClientData sampleServerResponse = new("getAnalytics", null, null, expectedData, user);
            clientSessionManagerB.OnDataReceived(_serializer.Serialize(sampleServerResponse));
            SessionAnalytics actualData = newUX.sessionAnalytics;
            CollectionAssert.AreEqual(expectedData.chatCountForEachUser, actualData.chatCountForEachUser);
            CollectionAssert.AreEqual(expectedData.insincereMembers, actualData.insincereMembers);
            CollectionAssert.AreEqual(expectedData.userCountAtAnyTime, actualData.userCountAtAnyTime);
        }

        [Test]
        public void ModuleInitialisations_ClientModuleInitialisations_InitialisesRequiredModules()
        {
            _testWhiteBoard.isWhiteBoardInitialised = false;
            _ = SessionManagerFactory.GetClientSessionManager(_testCommunicator, _testWhiteBoard);
            Assert.IsTrue(_testWhiteBoard.isWhiteBoardInitialised);
        }

        [Test]
        [Description("The session manager will remove the user from session and broadcast the modified" + 
                        "session object via networking")]
        public void OnClientLeft_SuddenClientDeparture_RemovesClientFromSession()
        {
            int userSize = 10;
            List<UserData> users = Utils.GenerateUserData(userSize);
            AddUsersAtServer(users);
            UserData leavingUser = users[userSize - 1];
            serverSessionManager.OnClientLeft(leavingUser.userID.ToString());
            ServerToClientData serverToClientData = _serializer.Deserialize<ServerToClientData>(_testCommunicator.sentData);
            users.RemoveAt(userSize - 1);
            Assert.AreEqual("removeClient", serverToClientData.eventType);
            Assert.AreEqual(users, serverToClientData.sessionData.users);
        }

        private void AddUserClientSide(string username, int userId, string ip = "192.168.1.1", string port = "8080")
        {
            UserData userData = new(username, userId);
            IUXClientSessionManager _uxSessionManager = clientSessionManagerB;
            INotificationHandler _networkSessionManager = clientSessionManagerB;
            // Creating the user who joined
            ServerToClientData serverToClientData = new("removeClient", new(), null, null, userData);
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
        private TestContentServer _testContentServer;
        private TestWhiteBoard _testWhiteBoard;
    }
}