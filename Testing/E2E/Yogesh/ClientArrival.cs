/// <author>Yogesh</author>
/// <created>26/11/2021</created>

using System;
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
        private ICommunicator _clientCommunicator;
        private ICommunicator _serverCommunicator;
        private MeetingCredentials _meetingCredentials;
        private AuthViewModel _authViewModel;
        // private ChatViewModel _chatViewModel;

        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("isTesting", "true");
            
        }

        [OneTimeTearDown]
        public void Close()
        {
            Environment.SetEnvironmentVariable("isTesting", "false");
        }

        [Test]
        public void AuthCheck()
        {
            _authViewModel = new AuthViewModel();
            _authViewModel.SendForAuth("127.0.0.1", 8080, "Yogesh");
            
        }

        [Test]
        public void MoreThanOneUserAdded()
        {
            // _authViewModel.SendForAuth(_meetingCredentials.ipAddress, _meetingCredentials.port, "Mario");
            // Thread.Sleep(1000);
        }

    }
}