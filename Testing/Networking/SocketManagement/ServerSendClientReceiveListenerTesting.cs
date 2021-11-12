using System;
using System.Linq;
using System.Threading;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    
    [TestFixture]
    public class ServerSendListenerTesting
    {
        private ICommunicator _server;
        private ICommunicator _client2;
        private ICommunicator _client3;
        private static Random _random = new Random();
        
        [SetUp]
        public void Setup()
        {
            Thread.Sleep(300);
            // start the server
            _server = CommunicationFactory.GetCommunicator(false, true);
            string address = _server.Start();
            string[] s = address.Split(":");


            // client2 connection 
            _client2 = CommunicationFactory.GetCommunicator(true, true);
            string c2 = _client2.Start(s[0], s[1]);

            // client3 connection 
            _client3 = CommunicationFactory.GetCommunicator(true, true);
            string c3 = _client3.Start(s[0], s[1]);
            Thread.Sleep(50);
        }

        [TearDown]
        public void Dispose()
        {
            Thread.Sleep(500);
            // stop all clients 
            _client2.Stop();
            _client3.Stop();

            // stop the server
            _server.Stop();
        }

        [Test, Category("pass")]
        public void BroadCastServerSendClientReceiveListenerTest()
        {
            String msg = "BroadCastMessage: clients ";
            _server.Send(msg, "S");
            Thread.Sleep(300);
            Packet p2 = _client2.FrontPacket();
            Assert.AreEqual(msg, p2.SerializedData);
            Packet p3 = _client3.FrontPacket();
            Assert.AreEqual(msg, p3.SerializedData);
        }

        [Test, Category("pass")]
        public void PrivateServerSendClientReceiveListenerTest()
        {
            String msg = "PrivateMessage";
            _server.Send(msg, "W", "1");

            Thread.Sleep(300);
            Packet p2 = _client2.FrontPacket();
            Assert.AreEqual(msg, p2.SerializedData);
        }

        [Test, Category("pass")]
        public void FragmentationServerSendClientReceiveListenerTest()
        {
            string text = "";
            int length = 1030;
            const string chars = "ABCDFGHIJKLMNOPQRSTUVWXYZ0123456789";
            text = new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());

            _server.Send(text, "C", "1");

            Thread.Sleep(300);
            Packet p2 = _client2.FrontPacket();
            Assert.AreEqual(text, p2.SerializedData);
        }
    }
}