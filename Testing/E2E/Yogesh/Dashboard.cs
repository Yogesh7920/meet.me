using System;
using System.IO;
using Client.ViewModel;
using Content;
using Dashboard;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    
    [TestFixture]
    public class Dashboard
    {
        private ISerializer _serializer;
        private ServerSessionManager _serverSessionManager;
        private ClientSessionManager _clientSessionManager;
        private ContentServer _contentServer;
        
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _serializer = new Serializer();
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            _contentServer = new ContentServer();
        }

        void AddChat(string message)
        {
            var chatViewModel = new ChatViewModel();
            chatViewModel.SendChat(message, -1);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            _contentServer.Receive(serializedData);
        }
            
        [Test]
        [TestCase("127.0.0.1", 8080, "Yogesh", false)]
        [TestCase("127.0.0.1", 8080, "    ", true)] // username cannot be space.
        [TestCase("127.0.0.1", 8080, null, true)] // username cannot be null
        public void ClientArrival(string ip, int port, string username, bool error)
        {
            var authViewModel = new AuthViewModel();
            var valid = authViewModel.SendForAuth("127.0.0.1", 8080, username);
            Assert.AreEqual(valid, !error);
            if (error) return;
            var serializedData = File.ReadAllText("networking_output.json");
            var dataSent = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(dataSent.eventType, "addClient");
            Assert.AreEqual(dataSent.username, username);
            _serverSessionManager.OnDataReceived(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var dataReceived = _serializer.Deserialize<ServerToClientData>(serializedData);
            Assert.AreEqual(dataReceived.eventType, "addClient");
            var users = dataReceived.sessionData.users;
            Assert.AreEqual(users[0].username, username);
            _clientSessionManager.OnDataReceived(serializedData);
        }

        [Test]
        public void GetTelemetry()
        {
            
            var authViewModel = new AuthViewModel();
            authViewModel.SendForAuth("127.0.0.1", 8080, "ABC");
            var serializedData = File.ReadAllText("networking_output.json");
            _serverSessionManager.OnClientJoined(1);
            _serverSessionManager.OnDataReceived(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            _clientSessionManager.OnDataReceived(serializedData);
            
            var dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.UpdateVM();
            serializedData = File.ReadAllText("networking_output.json");
            var dataSent = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(dataSent.eventType, "getAnalytics");
            _serverSessionManager.OnDataReceived(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var dataReceived = _serializer.Deserialize<ServerToClientData>(serializedData);
            Assert.AreEqual(dataReceived.eventType, "getAnalytics");
            _clientSessionManager.OnDataReceived(serializedData);
        }

        [Test]
        public void GetSummary()
        {
            _clientSessionManager.SetUser("Yogesh");
            _clientSessionManager.GetSummary();
            var serializedData = File.ReadAllText("networking_output.json");
            var dataSent = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(dataSent.eventType, "getSummary");
            
            AddChat("Hello");
            AddChat("Hi");
            
            var serverSessionManager =
                new ServerSessionManager(CommunicationFactory.GetCommunicator(false), _contentServer);
            
            serverSessionManager.OnDataReceived(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var dataReceived = _serializer.Deserialize<ServerToClientData>(serializedData);
            Assert.AreEqual(dataReceived.eventType, "getSummary");
            _clientSessionManager.OnDataReceived(serializedData);
            
        }

        [Test]
        public void ClientDeparture()
        {
            GetSummary();
            GetTelemetry();

            var homePageViewModel = new HomePageViewModel();
            homePageViewModel.LeftClient();
            var serializedData = File.ReadAllText("networking_output.json");
            var dataSent = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(dataSent.eventType, "removeClient");
            _serverSessionManager.OnDataReceived(serializedData);
            
            serializedData = File.ReadAllText("networking_output.json");
            var dataReceived = _serializer.Deserialize<ServerToClientData>(serializedData);
            Assert.AreEqual(dataReceived.eventType, "endMeet"); // Since the only user is leaving
            _clientSessionManager.OnDataReceived(serializedData);
        }

    }
}