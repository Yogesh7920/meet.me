using NUnit.Framework;
using Networking;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace Testing
{
    [TestFixture]
    public class ClientSendServerReceiveListenerTesting
    {
        private ICommunicator _server;
        private ICommunicator _client2;
        private ICommunicator _client3;
        private static Random _random = new Random();
        
        [SetUp]
        public void Setup()
        {
            Thread.Sleep(1000);
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
        }

        [TearDown]
        public void Dispose()
        {
            // stop all clients
            _client2.Stop();
            _client3.Stop();

            // stop the server
            _server.Stop();
        }

        [Test, Category("pass")]
        public void SinglePacketSingleClientSendServerReceiveListenerTest()
        {
            String msg = "msg from client3 to server ";
            String identifier = "S";
            _client3.Send(msg, identifier);

            Thread.Sleep(1000);

            Packet p = _server.FrontPacket();
            Assert.AreEqual(msg, p.SerializedData);
        }

        [Test, Category("pass")]
        public void MultiplePacketSingleClientSendServerReceiveListenerTest()
        {
            String msg1 = "first packet ";
            String identifier = "S";
            String msg2 = "second packet ";
            _client3.Send(msg1, identifier);
            _client3.Send(msg2, identifier);
            Thread.Sleep(300);

            Packet p1 = _server.FrontPacket();
            Assert.AreEqual(msg1, p1.SerializedData);
            
            Packet p2 = _server.FrontPacket();
            Assert.AreEqual(msg2, p2.SerializedData);
        }

        [Test, Category("pass")]
        public void MultipleClientSendServerReceiveListenerTest()
        {
            String msg1 = "msg from client3 ";
            String identifier1 = "S";
            _client2.Send(msg1, identifier1);
            String msg2 = "msg from client2 ";
            String identifier2 = "C";
            _client3.Send(msg2, identifier2);
            Thread.Sleep(1000);
            //Result can vary according to thread
            Packet p1 = _server.FrontPacket();
            Assert.AreEqual(msg2, p1.SerializedData);
            Packet p2 = _server.FrontPacket();
            Assert.AreEqual(msg1, p2.SerializedData);
        }

        [Test, Category("pass")]
        public void FragmentationClientSendServerReceiveListenerTest()
        {
            string text = "";
            int length = 4030;
            const string chars = "ABCDFGHIJKLMNOPQRSTUVWXYZ0123456789";
            text = new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
   
            _client2.Send(text, "C");
            Thread.Sleep(1000);
            Packet p2 = _server.FrontPacket();
            Assert.AreEqual(text, p2.SerializedData);
        }
    }
}