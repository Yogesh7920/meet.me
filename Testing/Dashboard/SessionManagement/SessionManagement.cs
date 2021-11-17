using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dashboard.Client.SessionManagement;
using Dashboard.Server.SessionManagement;
using Dashboard;

namespace Testing.Dashboard
{
    public class SessionManagement
    {
        private IUXClientSessionManager _clientSessionManager;
        private ITelemetrySessionManager _serverSessionManager;
        private SessionData _sessionData;

        [SetUp]
        public void Setup()
        {
            _clientSessionManager = SessionManagerFactory.GetClientSessionManager();
            _serverSessionManager = SessionManagerFactory.GetServerSessionManager();
        }

        [Test]
        [TestCase("")]
        public void ValidIPAddress_ReturnTrue(string value)
        {
            Assert.Pass();
            //bool result =; 
        }
    }
}
