using System.Threading;
using Client.ViewModels;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    public class ClientArrival
    {
        private ServerSessionManager _serverSessionManager;
        private ICommunicator _clientCommunicator;
        private ICommunicator _serverCommunicator;
        private MeetingCredentials _meetingCredentials;
        private AuthViewModel _authViewModel;

        [OneTimeSetUp]
        public void Setup()
        {
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            _clientCommunicator = CommunicationFactory.GetCommunicator();
            _serverCommunicator = CommunicationFactory.GetCommunicator(false);
            _meetingCredentials = _serverSessionManager.GetPortsAndIPAddress();
            _authViewModel = new AuthViewModel();
            _authViewModel.SendForAuth(_meetingCredentials.ipAddress, _meetingCredentials.port, "Yogesh");
            Thread.Sleep(1000);
        }

        [OneTimeTearDown]
        public void Close()
        {
            _clientCommunicator.Stop();
            _serverCommunicator.Stop();
        }

        [Test]
        public void UserAdded()
        {
            var clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            var user = clientSessionManager._clientSessionData.users[0];
            Assert.AreEqual(user.username, "Yogesh");
        }

        [Test]
        public void MoreThanOneUserAdded()
        {
            _authViewModel.SendForAuth(_meetingCredentials.ipAddress, _meetingCredentials.port, "Mario");
            Thread.Sleep(1000);
            var clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            var users = clientSessionManager._clientSessionData.users;
            Assert.AreEqual(users.Count, 2);
        }

    }
}