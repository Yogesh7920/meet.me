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

namespace Testing.Dashboard
{
    public class SessionManagementTest
    {
        private IUXClientSessionManager _clientSessionManager;
        private ServerSessionManager _serverSessionManager;
        private TestCommunicator _communicatorTest;
        private SessionData _sessionData;

        [SetUp]
        public void Setup()
        {
            _communicatorTest = new();
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager();
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

        [Test]
        public void compareUsers()
        {
            List<UserData> users = new List<UserData>();
            UserData a, b, c;

            a = new("a", 1);
            b = new("b", 2);
            c = new("c", 3);

            users.Add(a);
            users.Add(b);
            users.Add(c);

            UserData r = new("a", 1);

            foreach(var user in users)
            {
                if(user.Equals(r))
                {
                    Console.WriteLine("Found the user!");
                }
            }
            
        }
    }
}
