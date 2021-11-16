using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class SendSocketListenerServerTesting
    {
        private IQueue _queueS;
        private IQueue _queueR;
        private Machine _server;
        private SendSocketListenerServer _sendSocketListenerServer;
        private ReceiveSocketListener _receiveSocketListener;
        private TcpClient _serverSocket;
        private TcpClient _clientSocket;
        private  Dictionary<string, TcpClient> _clientIdSocket;

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
            
            _queueS = new Queue();
            _queueS.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _clientIdSocket = new();
            _sendSocketListenerServer = new SendSocketListenerServer(_queueS, _clientIdSocket);
            _sendSocketListenerServer.Start();
            
            _queueR = new Queue();
            _queueR.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _receiveSocketListener = new ReceiveSocketListener(_queueR, _clientSocket);
            _receiveSocketListener.Start();
            
            Task t2 = Task.Run(() =>
            {
                _serverSocket = serverSocket.AcceptTcpClient();
                _clientIdSocket["1"] = _serverSocket;
            });
            Task.WaitAll(t1, t2);
        }

        [Test]
        public void SinglePacketServerSendTesting()
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
        public void BigPacketServerSendTesting()
        {
            string whiteBoardData = NetworkingGlobals.GetRandomString(1500);
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
         
        [TearDown]
        public void TearDown()
        {
            _serverSocket.Close();
            _receiveSocketListener.Stop();
            _sendSocketListenerServer.Stop();
            
            _clientSocket.Close();
        }
    }
}