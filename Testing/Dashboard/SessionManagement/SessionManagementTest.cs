using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Dashboard;
using Networking;
using Testing.Dashboard.SessionManagement.TestModules;
using System.Net.Sockets;

namespace Testing.Dashboard.SessionManagement
{
    public class SessionManagementTest
    {
        private IUXClientSessionManager _clientSessionManager;
        private ServerSessionManager _serverSessionManager;
        private TestCommunicator _communicatorTest;

        [SetUp]
        public void Setup()
        {
            _communicatorTest = new();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager(_communicatorTest);
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager(_communicatorTest);
        }

        [Test]
        [TestCase("192.168.1.1:8080")]
        [TestCase("195.148.23.101:8585")]
        [TestCase("223.152.44.2:2222")]
        public void GetPortsAndIPAddress_ValidAddress_ReturnsTrue(string inputMeetAddress)
        {
            IUXServerSessionManager serverSessionManager = _serverSessionManager;
            _communicatorTest.meetAddress = inputMeetAddress;
            MeetingCredentials meetCreds = serverSessionManager.GetPortsAndIPAddress();
            string returnedMeetAddress = meetCreds.ipAddress + ":" + meetCreds.port.ToString();
            Assert.AreEqual(_communicatorTest.meetAddress, returnedMeetAddress);
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
            _communicatorTest.meetAddress = inputMeetAddress;
            MeetingCredentials meetCreds = serverSessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(meetCreds, null);
        }

        //[Test]
        //[TestCase("192.168.20.1:8080","192.168.20.1",8080,"Jake Vickers")]
        //[TestCase("192.168.201.4:480", "192.168.201.4", 480, "Antonio")]
        //public void AddClient_ValidCredentials_ReturnsTrue(string meetAddress, string ipAddress, int port, string username)
        //{
        //    _communicatorTest.meetAddress = meetAddress;
        //    bool clientAdded =_clientSessionManager.AddClient(ipAddress, port, username);
        //    Assert.AreEqual(true, clientAdded);
        //}

        //[Test]
        //[TestCase("","",51,"")]
        //[TestCase(null, null, null, null)]
        //[TestCase(null,"162.212.3.1",20,"Chang Jia-han")]
        //[TestCase("192.168.201.4:480", "192.230.201.4", 480, "Antonio")]
        //[TestCase("192.168.20.1:8080", "192.168.20.1", 8081, "Jake Vickers")]
        //public void AddClient_InvalidCredentials_ReturnsFalse(string meetAddress, string ipAddress, int port, string username)
        //{
        //    _communicatorTest.meetAddress = meetAddress;
        //    bool clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
        //    Assert.AreEqual(false, clientAdded);
        //}

        //[Test]
        //[TestCase("192.168.1.1",8080,"Jake")]
        //public void ClientArrivalProcedure_ClientArrives_BroadcastsNewUser(string ipAddress, int port, string username)
        //{
        //    bool clientAdded = _clientSessionManager.AddClient(ipAddress, port, username);
        //    _serverSessionManager.OnClientJoined<TcpClient>(null);
        //    _serverSessionManager.OnDataReceived(_communicatorTest.transferredData);

        //    UserData updatedUser = _clientSessionManager.GetUser();
        //    Assert.AreEqual(updatedUser.username, username);
        //    Assert.NotNull(updatedUser.userID);
        //}


        
    }
}