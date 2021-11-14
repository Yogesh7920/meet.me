using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class SendSocketListenerClientTesting
    {
        private IQueue _queueS;
        private IQueue _queueR;
        private Machine _server;
        private SendSocketListenerClient _sendSocketListenerClient;
        private ReceiveSocketListener _receiveSocketListener;
        private const int Threshold = 1025;
        private string Message => NetworkingGlobals.GetRandomString();
        private TcpClient _serverSocket;
        private TcpClient _clientSocket;
        
        private string GetMessage(Packet packet)
        {
            string msg = packet.ModuleIdentifier;
            msg += ":";
            msg += packet.SerializedData;
            msg += "EOF";
            return msg;
        }

        [SetUp]
        public void StartSendSocketListenerClient()
        {
            _server = new FakeServer();
            string[] address = _server.Communicator.Start().Split(":");
            int port = Int32.Parse(address[1]);
            IPAddress ip = IPAddress.Parse(address[0]);
            _server.Communicator.Stop();
            TcpListener serverSocket = new TcpListener(ip, port);
            serverSocket.Start();
            _clientSocket = new TcpClient();
            _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            Task t1 = Task.Run(() =>
            {
                _clientSocket.Connect(ip, port);
            });
            Task t2 = Task.Run(() =>
            {
                _serverSocket = serverSocket.AcceptTcpClient();
            });
            Task.WaitAll(t1, t2);
            _queueS = new Queue();
            _queueS.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _sendSocketListenerClient = new SendSocketListenerClient(_queueS, _clientSocket);
            _sendSocketListenerClient.Start();
            
            _queueR = new Queue();
            _queueR.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _receiveSocketListener = new ReceiveSocketListener(_queueR, _serverSocket);
            _receiveSocketListener.Start();
        }

        [Test]
        public void SinglePacketClientSendTesting()
        {
            string whiteBoardData = "hello ";
            Packet whiteBoardPacket = new Packet{ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);
            
            while (_queueR.IsEmpty()) { }
            Packet packet = _queueR.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
            });
        }
        
        [Test]
        public void BigPacketClientSendTesting()
        {
            string whiteBoardData = NetworkingGlobals.GetRandomString(4000);
            Packet whiteBoardPacket = new Packet{ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);
            
            while (_queueR.IsEmpty()) { }
            Packet packet = _queueR.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            });
        }

        [Test]
        public void MultiplePacketClientSendTesting()
        { 
            for (int i = 1; i <= 10; i++)
            {
                string whiteBoardData = "packet"+i.ToString();
                Packet whiteBoardPacket = new Packet{ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
                _queueS.Enqueue(whiteBoardPacket);
            }
            
         
            Thread.Sleep(100);
            for (int i = 1; i <= 10; i++)
            {
                string whiteBoardData = "packet"+i.ToString();
                Packet packet = _queueR.Dequeue();
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            }
        }
        [TearDown]
        public void TearDown()
        {
            _clientSocket.Close();
            _sendSocketListenerClient.Stop();
            _receiveSocketListener.Stop();
            _serverSocket.Close();
        }
    }
}