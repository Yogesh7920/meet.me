using System;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    public class ServerE2E
    {
        private ISerializer _serializer;
        
        
        [OneTimeSetUp]
        public void Setup()
        {
            _serializer = new Serializer();
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
        }

        public void SendAllContent()
        {
            
        }

        public void SendWhiteboard()
        {
            
        }

        public void SendScreenShare()
        {
            
        }

        public void SendUsers()
        {
            
        }

        public void SendTelemetry()
        {
            
        }

        public void SendSummary()
        {
            
        }

    }
}