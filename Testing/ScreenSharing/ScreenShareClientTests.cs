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
    public class ScreenShareClientTests
    {
   
        private TestCommunicator communicator;
        private TestUX ux;

        [SetUp]
        public void Setup()
        {
            communicator= new TestCommunicator();
            ux = new TestUX();
        }

        [Test]
        [TestCase("1","Neeraj")]
        [TestCase("2", "Manas")]
        [TestCase("3", "Vinay")]
        public void SetUser(string uid,string uname)
        {
            var ssClient = ScreenShareFactory.GetScreenShareClient(true, communicator);
            
            ssClient.SetUser(uid, uname);
            Assert.AreEqual(ssClient.userId, uid);
            Assert.AreEqual(ssClient.userName, uname);

            ssClient.Dispose();
        }

        [Test]
        [TestCase("1","Neeraj",1,null)]
        [TestCase("2", "Manas",1,null)]
        [TestCase("3", "Vinay",1,null)]
        public void Send(string uid,string uname,int mtype, byte[] b)
        {
            var ssClient = ScreenShareFactory.GetScreenShareClient(true, communicator);
            ISerializer serializer = new Serializer();
            SharedScreen m = new SharedScreen(uid,uname,mtype,b);
            ssClient.Send(m);
            SharedScreen data = serializer.Deserialize<SharedScreen>(communicator.sentData);
            Assert.AreEqual(data.userId, uid);
            Assert.AreEqual(data.userName, uname);
            Assert.AreEqual(data.messageType, mtype);

            ssClient.Dispose();
        }

        [Test]
        [TestCase("1", "Neeraj", 1, null)]
        [TestCase("2", "Manas", 0, null)]
        [TestCase("3", "Vinay", 1, null)]
        public void OnScreenRecieved(string uid, string uname, int mtype, byte[] b)
        {
            
            var ssClient = ScreenShareFactory.GetScreenShareClient(true, communicator);
            ISerializer serializer = new Serializer();
            ssClient.Subscribe(ux);
            SharedScreen m = new SharedScreen(uid, uname, mtype, b);
            string data = serializer.Serialize<SharedScreen>(m);

            ssClient.OnDataReceived(data);

            Thread.Sleep(1000);
            Assert.AreEqual(ux.UserId, uid);
            Assert.AreEqual(ux.UserName, uname);
            Assert.AreEqual(ux.MessageType, mtype);
            ssClient.Dispose();
        }

        [Test]
        [TestCase("1", "Neeraj")]
        [TestCase("2", "Manas")]
        [TestCase("3", "Vinay")]
        public void Capture(string uid, string uname)
        {
            var ssClient = ScreenShareFactory.GetScreenShareClient(true, communicator);
            ssClient.SetUser(uid, uname);
            SharedScreen message=ssClient.Capture();

            Assert.AreEqual(message.userId, uid);
            Assert.AreEqual(message.userName, uname);
            Assert.AreEqual(message.messageType, 1);

            ssClient.Dispose();
        }

        [Test]
        [TestCase("1", "Neeraj")]
        [TestCase("2", "Manas")]
        [TestCase("3", "Vinay")]
        public void OnTimeout(string uid, string uname)
        {
            var ssClient = ScreenShareFactory.GetScreenShareClient(true, communicator);
            ssClient.SetUser(uid, uname);
            ssClient.Subscribe(ux);
            ssClient.timer.Start();
            Thread.Sleep(20000);

            Assert.AreEqual(ux.UserId, uid);
            Assert.AreEqual(ux.UserName, uname);
            Assert.AreEqual(ux.MessageType, -2);

            ssClient.Dispose();

        }
    }
}
