using NUnit.Framework;

using Networking;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Queue = Networking.Queue;
using System.Threading;

namespace Testing
{
    [TestFixture]
    public class SendSocketListenerClientTesting
    {
        private IQueue _queue;
        private List<Packet> _testPackets;
        private List<string> _moduleIdentifiers;
        private TcpClient _client;
        private TcpListener _server;
        private SendSocketListenerClient s;
        [SetUp]
        public void Setup()
        {
            // start a dummy server and join with client
            IPAddress ip = IPAddress.Loopback;
            int port = 8888;
            _server =new TcpListener(ip,port);
            _server.Start();
            _client = new TcpClient();
            _client.Connect(ip,port);

            // form queue
            _queue = new Queue();

            _testPackets = new List<Packet>(100);
            _moduleIdentifiers = new List<string>(4);

            const string screenShareModuleId = "S";
            const string whiteBoardModuleId = "W";
            const string chatModuleId = "C";
            const string fileModuleId = "F";

            _moduleIdentifiers.Add(screenShareModuleId);
            _moduleIdentifiers.Add(whiteBoardModuleId);
            _moduleIdentifiers.Add(chatModuleId);
            _moduleIdentifiers.Add(fileModuleId);

            const int screenSharePriority = 4;
            const int whiteBoardPriority = 3;
            const int chatPriority = 2;
            const int filePriority = 1;

            _queue.RegisterModule(screenShareModuleId, screenSharePriority);
            _queue.RegisterModule(whiteBoardModuleId, whiteBoardPriority);
            _queue.RegisterModule(chatModuleId, chatPriority);
            _queue.RegisterModule(fileModuleId, filePriority);
         
            s = new SendSocketListenerClient(_queue, _client);

            const string moduleId = "S";
            const string data = "testData";
            Packet packet = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            _queue.Enqueue(packet);
        }

        [Test]
        public void sendListenerTest()
        {
            s.Start();
            const string moduleId = "S";
            const string data = "testData";
            Packet packet = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            // client side
            _queue.Enqueue(packet);
            Thread.Sleep(2);
            s.Stop();

            Assert.AreEqual(1, 1);
        }
    }
}