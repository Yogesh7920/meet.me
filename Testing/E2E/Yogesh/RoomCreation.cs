using Dashboard;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    public class RoomCreation
    {
        private ServerSessionManager _serverSessionManager;
        private ICommunicator _serverCommunicator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            _serverCommunicator = CommunicationFactory.GetCommunicator(false);
        }

        [OneTimeTearDown]
        public void Stop()
        {
            _serverCommunicator.Stop();
        }
        

        [Test]
        public void CheckValidMeetingCredentials()
        {
            var meetingCredentials = _serverSessionManager.GetPortsAndIPAddress();
            Assert.NotNull(meetingCredentials.ipAddress);
            Assert.NotNull(meetingCredentials.port);
        }
    }
}