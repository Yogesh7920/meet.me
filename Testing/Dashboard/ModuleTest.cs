using Dashboard;
using Dashboard.Server.SessionManagement;
using NUnit.Framework;
using Testing.Dashboard;
using Testing.Dashboard.TestModels;

namespace Testing
{
    public class DashboardModuleTests
    {
        private readonly TestCommunicator _testCommunicator = new();

        private readonly int validTests = 10;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RoomCreationTest()
        {
            IUXServerSessionManager _sessionManager = new ServerSessionManager(_testCommunicator);
            MeetingCredentials returnedMeetCreds;
            MeetingCredentials testMeetCreds;
            for (var i = 0; i < validTests; i++)
            {
                // Testing different combinations of valid IPs and Port number
                _testCommunicator.ipAddressAndPort = Utils.GenerateValidIPAndPort();
                testMeetCreds = Utils.generateMeetingCreds(_testCommunicator.ipAddressAndPort);
                returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
                Assert.AreEqual(testMeetCreds.port, returnedMeetCreds.port);
                Assert.AreEqual(testMeetCreds.ipAddress, returnedMeetCreds.ipAddress);
            }

            // Testing Invalid Test cases
            _testCommunicator.ipAddressAndPort = "";
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(null, returnedMeetCreds);

            _testCommunicator.ipAddressAndPort = null;
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(null, returnedMeetCreds);

            _testCommunicator.ipAddressAndPort = "abcd.192.1.2:8080";
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(null, returnedMeetCreds);

            _testCommunicator.ipAddressAndPort = "192.1.2:8080";
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(null, returnedMeetCreds);

            _testCommunicator.ipAddressAndPort = "abcdefg";
            returnedMeetCreds = _sessionManager.GetPortsAndIPAddress();
            Assert.AreEqual(null, returnedMeetCreds);
        }

        [Test]
        public void ClientArrivalClientSideTest()
        {
            Assert.Pass();
        }
    }
}