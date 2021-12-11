using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ScreenSharing;
using Networking;
using Testing.ScreenSharing.TestModels;
using System.Drawing;
using System.Threading;

namespace Testing.ScreenSharing
{
    class ScreenShareServerTests
    {
        private TestCommunicator communicator;

        [SetUp]
        public void Setup()
        {
            communicator = new TestCommunicator();
        }

        [Test]
        public void OnTimeout()
        {
            var ssServer = ScreenShareFactory.GetScreenShareServer(true, communicator);

            ssServer.timer.Start();
            Thread.Sleep(10100);
            Assert.AreEqual(ssServer.userId, "-");

            ssServer.Dispose();

        }

        [Test]
        [TestCase("1", "Neeraj", null)]
        [TestCase("2", "Manas", null)]
        [TestCase("3", "Vinay", null)]
        public void OnDataReceived(string uid,string uname,byte[] b)
        {
            var ssServer = ScreenShareFactory.GetScreenShareServer(true, communicator);
            SharedScreen m1 = new SharedScreen(uid,uname,1,b);
            ISerializer serializer = new Serializer();
            string data1=serializer.Serialize<SharedScreen>(m1);

            ssServer.OnDataReceived(data1);

            Thread.Sleep(100);
            string dataReceived1 = communicator.sentData;
            SharedScreen message1 = serializer.Deserialize<SharedScreen>(dataReceived1);

            Assert.AreEqual(message1.userId, uid);
            Assert.AreEqual(message1.userName, uname);
            Assert.AreEqual(message1.messageType, 1);

            SharedScreen m2 = new SharedScreen(uid, uname, 0, b);
            string data2 = serializer.Serialize<SharedScreen>(m2);

            ssServer.OnDataReceived(data2);

            Thread.Sleep(100);
            string dataReceived2 = communicator.sentData;
            SharedScreen message2 = serializer.Deserialize<SharedScreen>(dataReceived2);

            Assert.AreEqual(message2.userId, uid);
            Assert.AreEqual(message2.userName, uname);
            Assert.AreEqual(message2.messageType, 0);

            ssServer.Dispose();

        }
    }
}
