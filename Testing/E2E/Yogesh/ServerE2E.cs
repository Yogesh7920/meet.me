using System;
using System.Collections.Generic;
using System.IO;
using Content;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    [TestFixture]
    [NonParallelizable]
    public class ServerE2E
    {
        private ISerializer _serializer;
        private IContentServer _content;
        private ServerSessionManager _sessionManager;
        // private 


        [OneTimeSetUp]
        public void Setup()
        {
            _serializer = new Serializer();
            _content = ContentServerFactory.GetInstance();
            _sessionManager = SessionManagerFactory.GetServerSessionManager();
            
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
        }
        
        [Test]
        public void SendUsers()
        {
            
        }

        [Test]
        public void SendTelemetry()
        {
            
        }

        [Test]
        public void SendSummary()
        {
            
        }

        [Test]
        public void SendAllContent()
        {
            _content.SSendAllMessagesToClient(1);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<List<ChatContext>>(serializedData);
        }

        [Test]
        public void SendScreenShare()
        {
            
        }

    }
}