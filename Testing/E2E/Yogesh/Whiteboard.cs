using System;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    [TestFixture]
    public class Whiteboard
    {
        private ISerializer _serializer;

        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _serializer = new Serializer();
        }

        [Test]
        public void CreateShape()
        {
            
        }
    }
}