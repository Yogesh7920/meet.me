/// <author> Rajeev Goyal </author>
/// <created> 14/10/2021 </created>
/// <summary>
/// This file contains the interface for UX to access Client session manager's methods and fields.
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Dashboard.Server.Telemetry;
using Networking;
using NUnit.Framework;
using Testing.Dashboard.SessionManagement.TestModules;

namespace Testing.Dashboard.SessionManagement
{
    public class SessionManagementTest
    {
        private ClientSessionManager _clientSessionManager;
        private ClientSessionManager _clientSessionManagerLast;
        private ClientSessionManager _clientSessionManagerNew;

        private FakeCommunicator _fakeCommunicator;
        private FakeContentServer _fakeContentServer;
        private ISerializer _serializer;
        private ServerSessionManager _serverSessionManager;

        [SetUp]
        public void Setup()
        {
            _fakeCommunicator = new FakeCommunicator();
            _fakeContentServer = new FakeContentServer();
            _serializer = new Serializer();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager(_fakeCommunicator);
            _serverSessionManager =
                SessionManagerFactory.GetServerSessionManager(_fakeCommunicator, _fakeContentServer);
        }

        [Test]
        public void GetClientSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXClientSessionManager clientSessionManager1 = SessionManagerFactory.GetClientSessionManager();
            IUXClientSessionManager clientSessionManager2 = SessionManagerFactory.GetClientSessionManager();

            Assert.That(ReferenceEquals(clientSessionManager1, clientSessionManager2));
        }

        [Test]
        public void GetServerSessionManager_TwoInstancesCreated_MustHaveSameReference()
        {
            IUXServerSessionManager serverSessionManager1 = SessionManagerFactory.GetServerSessionManager();
            IUXServerSessionManager serverSessionManager2 = SessionManagerFactory.GetServerSessionManager();

            Assert.That(ReferenceEquals(serverSessionManager1, serverSessionManager2));
        }

        [Test]
        public void NotifyUX_SessionDataChanges_UXShouldBeNotified()
        {
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            fakeClientUX.sessionSummary = null;

            var users = Utils.GetUsers();
            //SessionData updatedSession = new SessionData();
            //updatedSession.users = users;
            _clientSessionManager.SetSessionUsers(users);

            _clientSessionManager.NotifyUXSession();

            CollectionAssert.AreEqual(users, fakeClientUX.sessionData.users);
        }

        [Test]
        [TestCase("192.168.1.1:8080")]
        [TestCase("195.148.23.101:8585")]
        [TestCase("223.152.44.2:2222")]
        public void GetPortsAndIPAddress_ValidAddress_ReturnsTrue(string inputMeetAddress)
        {
            IUXServerSessionManager serverSessionManager = _serverSessionManager;
            _fakeCommunicator.meetAddress = inputMeetAddress;
            var meetCreds = serverSessionManager.GetPortsAndIPAddress();
            var returnedMeetAddress = meetCreds.ipAddress + ":" + meetCreds.port;
            Assert.AreEqual(_fakeCommunicator.meetAddress, returnedMeetAddress);
        }

        [Test]
        [TestCase("")]
        [TestCase("256.0.1.3:8080")]
        [TestCase("2$.3$%.5:3512")]
        [TestCase("192.168.1.2.1")]
        [TestCase("192.168.1.1:70000")]
        [TestCase(null)]
        [TestCase("3ha8gh")]
        public void GetPortsAndIPAddress_InvalidAddress_ReturnsNull(string inputMeetAddress)
        {
            IUXServerSessionManager serverSessionManager = _serverSessionManager;
            _fakeCommunicator.meetAddress = inputMeetAddress;
            var meetCreds = serverSessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(meetCreds, null);
        }

        [Test]
        [TestCase("192.168.20.1:8080", "192.168.20.1", 8080, "Jake Vickers")]
        [TestCase("192.168.201.4:480", "192.168.201.4", 480, "Antonio")]
        public void AddClient_ValidCredentials_ReturnsTrue(string meetAddress, string ipAddress, int port,
            string username)
        {
            _fakeCommunicator.meetAddress = meetAddress;
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.AreEqual(true, clientAdded);
        }

        [Test]
        [TestCase("", "", 51, "")]
        [TestCase(null, null, null, null)]
        [TestCase(null, "162.212.3.1", 20, "Chang Jia-han")]
        [TestCase("192.168.201.4:480", "192.230.201.4", 480, "Antonio")]
        [TestCase("192.168.20.1:8080", "192.168.20.1", 8081, "Jake Vickers")]
        public void AddClient_InvalidCredentials_ReturnsFalse(string meetAddress, string ipAddress, int port,
            string username)
        {
            _fakeCommunicator.meetAddress = meetAddress;
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
            Assert.AreEqual(false, clientAdded);
        }

        [Test]
        [TestCase("192.168.1.1", 8080, "Jake")]
        [TestCase("192.168.1.1", 8080, "Lake")]
        [TestCase("192.168.1.1", 8080, "Bake")]
        public void ClientArrivalProcedure_ClientArrives_BroadcastsNewUser(string ipAddress, int port, string username)
        {
            Console.WriteLine("Session Before\n\t" + _clientSessionManager.GetSessionData());
            var clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);

            _serverSessionManager.OnClientJoined<TcpClient>(null);
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Console.WriteLine("Session After\n\t" + _clientSessionManager.GetSessionData());
            //UserData updatedUser = _clientSessionManager.GetUser();
            //Assert.AreEqual(updatedUser.username, username);
            //Assert.NotNull(updatedUser.userID);
        }

        [Test]
        [TestCase("Jake")]
        public void AddClientProcedureServerSide_ClientArrives_NewClientAddedToServer(string username)
        {
            ClientToServerData clientToServerData = new("addClient", username);
            var serializedData = _serializer.Serialize(clientToServerData);

            _serverSessionManager.OnClientJoined<TcpClient>(null);
            _serverSessionManager.OnDataReceived(serializedData);

            var serverToClientData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            var receiveduser = serverToClientData.GetUser();

            Assert.AreEqual(serverToClientData.eventType, "addClient");
            Assert.AreEqual(receiveduser.username, username);
            Assert.NotNull(receiveduser.userID);
        }

        [Test]
        public void AddClientProcedureServerSide_MultipleClientsArrives_UsersAddedToServerSession()
        {
            // Clients that arrives are added to the server side
            var users = Utils.GetUsers();
            AddUsersToServerSession(users);

            // The updated session data which includes new users is now sent from server to the client side
            // the deserializedData.sessionData is the updated session received from the server 
            var deserializedData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            var returnedSessionData = deserializedData.sessionData;

            // The recieved session must not be null and have the same users that were added
            Assert.NotNull(returnedSessionData);
            CollectionAssert.AreEqual(returnedSessionData.users, users);
            Assert.AreEqual("addClient", deserializedData.eventType);
            Assert.AreEqual(users.Count, _fakeCommunicator.userCount);
        }

        [Test]
        public void UpdatingSessionDataOnArrival_ClientArrives_ClientSessionUpdated()
        {
            // Client session managers for the nth and n+1 th user respectively
            _clientSessionManagerLast = new ClientSessionManager(_fakeCommunicator);
            _clientSessionManagerNew = new ClientSessionManager(_fakeCommunicator);

            var serverSession = Utils.GetSessionData();
            // nth user
            var indexLastUser = serverSession.users.Count - 1;
            var lastUser = serverSession.users[indexLastUser];

            // Till now, the nth user has arrived and the server session data has been updated
            // Now, the server will send the new session to the client side to update it
            ServerToClientData serverToClientData = new("addClient", serverSession, null, null, lastUser);
            var serializedData = _serializer.Serialize(serverToClientData);

            // Updating the client side session for the nth user
            _clientSessionManagerLast.OnDataReceived(serializedData);

            // The (n+1)th user arrives and the server session data is updated
            UserData newUser = new("Yuzuhiko", serverSession.users.Count + 1);
            serverSession.AddUser(newUser);

            // Server Notifies the Client side about the addition of the new user
            ServerToClientData serverToClientDataNew = new("addClient", serverSession, null, null, newUser);
            var serializedDataNew = _serializer.Serialize(serverToClientDataNew);

            // Updating the already present nth users session
            _clientSessionManagerLast.OnDataReceived(serializedDataNew);

            // Updating the new user's session
            _clientSessionManagerNew.OnDataReceived(serializedDataNew);

            // Assertion to check if both nth and the (n+1)th user have the same session
            Assert.NotNull(_clientSessionManagerLast.GetUser());
            Assert.NotNull(_clientSessionManagerNew.GetUser());
            Assert.NotNull(_clientSessionManagerLast.GetSessionData());
            Assert.NotNull(_clientSessionManagerNew.GetSessionData());
            CollectionAssert.AreEqual(_clientSessionManagerLast.GetSessionData().users,
                _clientSessionManagerNew.GetSessionData().users);
            Assert.AreEqual(_clientSessionManagerLast.GetSessionData().users.Count, serverSession.users.Count);
        }

        [Test]
        public void GetSummary_RequestSummary_ReturnsSummaryAndNotifyUX()
        {
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            fakeClientUX.sessionSummary = null;

            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> {user});
            AddUsersToServerSession(new List<UserData> {user});

            _clientSessionManager.GetSummary();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            var clientSummary = _clientSessionManager.GetStoredSummary();
            var serverSummary = _serverSessionManager.GetStoredSummary();

            Assert.NotNull(clientSummary);
            Assert.NotNull(serverSummary);
            Assert.NotNull(fakeClientUX.sessionSummary);
        }

        [Test]
        public void GetAnalytics_RequestAnalytics_ReturnSessionAnalyticsAndNotifyUX()
        {
            FakeClientUX fakeClientUX = new(_clientSessionManager);
            FakeTelemetry fakeTelemetry = new(_serverSessionManager);

            UserData user = new("Jake Vickers", 1);

            _clientSessionManager.SetUser(user.username, user.userID);
            _clientSessionManager.SetSessionUsers(new List<UserData> {user});
            AddUsersToServerSession(new List<UserData> {user});

            _clientSessionManager.GetAnalytics();
            var serverToClientData = new ServerToClientData("getAnalytics", null, null, new SessionAnalytics(), user);
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            Assert.NotNull(_clientSessionManager.GetStoredAnalytics());
            Assert.NotNull(fakeClientUX.sessionAnalytics);
        }

        [Test]
        public void RemoveClient_ClientDeparture_UserRemovedFromServerAndClientSide()
        {
            var users = Utils.GetUsersSet2();
            AddUsersToServerSession(users);
            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // The last user in the list departs
            users.Remove(users.Last());
            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            var recData = _serializer.Deserialize<ServerToClientData>(_fakeCommunicator.transferredData);
            Console.WriteLine("USERS: " + recData.sessionData);
            Console.WriteLine("SUSERS: " + _serverSessionManager.GetSessionData());

            Assert.Null(_clientSessionManager.GetUser());
            Assert.Null(_clientSessionManager.GetSessionData());
            CollectionAssert.AreEqual(users, _serverSessionManager.GetSessionData().users);
        }

        [Test]
        public void EndMeet_MeetingEnded_UXNotified()
        {
            FakeClientUX fakeClientUx = new(_clientSessionManager);
            FakeServerUX fakeServerUX = new(_serverSessionManager);

            fakeClientUx.meetingEnded = false;
            fakeServerUX.meetingEnded = false;

            var users = Utils.GetUsers();
            AddUsersToServerSession(users);
            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            _clientSessionManager.EndMeet();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.AreEqual(fakeServerUX.meetingEnded, true);
            Assert.AreEqual(fakeClientUx.meetingEnded, true);
            Assert.AreEqual(_serverSessionManager.summarySaved, true);
        }

        [Test]
        public void OnDataReceivedServerSide_SendingNullData_TraceAndReturn()
        {
            try
            {
                _serverSessionManager.OnDataReceived(null);
            }
            catch (Exception e)
            {
                Assert.Fail("OnDataReceived failed: " + e);
            }
        }

        [Test]
        public void OnDataReceivedClientSide_SendingNullData_TraceAndReturn()
        {
            try
            {
                _clientSessionManager.OnDataReceived(null);
            }
            catch (Exception e)
            {
                Assert.Fail("OnDataReceived failed: " + e);
            }
        }

        [Test]
        public void OnClientLeft_ClientDisconnects_UserRemoved_ServerSessionChanged()
        {
            // Adding the users to the session and the client side
            var users = Utils.GetUsers();
            AddUsersToServerSession(users);
            _clientSessionManager.SetUser(users.Last().username, users.Last().userID);
            _clientSessionManager.SetSessionUsers(users);

            // removing the last user from the meet because of the disconnection
            var disconnectedUser = users.Last();
            users.Remove(users.Last());

            // The client disconnects and the client side is notified
            _serverSessionManager.OnClientLeft(disconnectedUser.userID.ToString());
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            // Check if the session on the server side was updated and the user and session data on the client side are removed.
            CollectionAssert.AreEqual(users, _serverSessionManager.GetSessionData().users);
            Assert.Null(_clientSessionManager.GetSessionData());
            Assert.Null(_clientSessionManager.GetUser());
        }

        [Test]
        public void EndMeet_LastUserLeaves_MeetingShouldEnd()
        {
            FakeClientUX fakeClientUx = new(_clientSessionManager);
            FakeServerUX fakeServerUX = new(_serverSessionManager);

            fakeClientUx.meetingEnded = false;
            fakeServerUX.meetingEnded = false;

            List<UserData> users = new();
            users.Add(new UserData("Justin", 1));

            AddUsersToServerSession(users);
            _clientSessionManager.SetSessionUsers(users);
            _clientSessionManager.SetUser(users[0].username, users[0].userID);

            _clientSessionManager.RemoveClient();
            _serverSessionManager.OnDataReceived(_fakeCommunicator.transferredData);
            _clientSessionManager.OnDataReceived(_fakeCommunicator.transferredData);

            Assert.AreEqual(fakeServerUX.meetingEnded, true);
            Assert.AreEqual(fakeClientUx.meetingEnded, true);
            Assert.AreEqual(_serverSessionManager.summarySaved, true);
        }

        public void AddUsersToServerSession(List<UserData> users)
        {
            for (var i = 0; i < users.Count; ++i)
            {
                ClientToServerData clientToServerData = new("addClient", users[i].username);
                var serializedData = _serializer.Serialize(clientToServerData);

                _serverSessionManager.OnClientJoined<TcpClient>(null);
                _serverSessionManager.OnDataReceived(serializedData);
            }
        }
    }
}