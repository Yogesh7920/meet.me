/// <author>Yogesh</author>
/// <created>26/11/2021</created>

using System.Threading;
using Client.ViewModel;
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
        private ChatViewModel _chatViewModel;

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
            Thread.Sleep(1000);
            var users = _chatViewModel.Users;
            var added = users.Values.Contains("Yogesh");
            Assert.IsTrue(added);
        }

        [Test]
        public void MoreThanOneUserAdded()
        {
            _authViewModel.SendForAuth(_meetingCredentials.ipAddress, _meetingCredentials.port, "Mario");
            Thread.Sleep(1000);
        }

    }
}