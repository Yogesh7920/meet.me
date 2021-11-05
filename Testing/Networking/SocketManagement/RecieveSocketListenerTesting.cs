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
    public class RecieveSocketListenerTesting
    {
        private RecieveSocketListener r;
        private IQueue _queueR;
        private IQueue _queueS;
        private List<Packet> _testPackets;
        private List<string> _moduleIdentifiers;
        private TcpClient _client,_socket;
        private TcpListener _server;
        private SendSocketListenerClient s;

        [SetUp]
        public void Setup()
        {
            // start a dummy server and join with client
            IPAddress ip = IPAddress.Loopback;
            int port = 8882;
            _server = new TcpListener(ip, port);
            _server.Start();
            _client = new TcpClient();
            _client.Connect(ip, port);
            _socket = _server.AcceptTcpClient();
            // form queue Recieveing side 
            _queueR = new Queue();
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
            _queueR.RegisterModule(screenShareModuleId, screenSharePriority);
            _queueR.RegisterModule(whiteBoardModuleId, whiteBoardPriority);
            _queueR.RegisterModule(chatModuleId, chatPriority);
            _queueR.RegisterModule(fileModuleId, filePriority);
            r = new RecieveSocketListener(_queueR, _socket);

            //form queue Sending side
            _queueS = new Queue();
            _queueS.RegisterModule(screenShareModuleId, screenSharePriority);
            _queueS.RegisterModule(whiteBoardModuleId, whiteBoardPriority);
            _queueS.RegisterModule(chatModuleId, chatPriority);
            _queueS.RegisterModule(fileModuleId, filePriority);
            s = new SendSocketListenerClient(_queueS, _client);
            const string moduleId = "S";
            const string data = "testData";
            Packet packet = new Packet { ModuleIdentifier = moduleId, SerializedData = data };
            _queueS.Enqueue(packet);

        }

        [Test]
        public void Test1()
        {
            r.Start();
            Thread.Sleep(1);
            s.Start();
            Thread.Sleep(1);
            s.Stop();
            Thread.Sleep(1);
            r.Stop();
            Packet p = _queueR.Dequeue();
            Console.WriteLine(p.ModuleIdentifier);
            Console.WriteLine(p.SerializedData);
            Assert.AreEqual(1,1);
        }
    }
}