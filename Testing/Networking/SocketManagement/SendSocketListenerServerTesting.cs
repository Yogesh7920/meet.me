/// <author>Tausif Iqbal</author>
/// <created>01/11/2021</created>
/// <summary>
/// This file covers the unit tests for for the class SendSocketListenerServer.
/// </summary>

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
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("TEST_MODE", "UNIT");
        }

        [SetUp]
        public void StartSendSocketListenerClient()
        {
            _server = new FakeServer();
            var address = _server.Communicator.Start().Split(":");
            _port = int.Parse(address[1]);
            _ip = IPAddress.Parse(address[0]);
            _server.Communicator.Stop();
            _serverListener = new TcpListener(_ip, _port);
            _serverListener.Start();
            _clientIdSocket = new Dictionary<string, TcpClient>();
            var t1 = Task.Run(() =>
            {
                _clientSocket1 = new TcpClient();
                _clientSocket1.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                _clientSocket1.Connect(_ip, _port);
            });

            var t2 = Task.Run(() =>
            {
                _serverSocket1 = _serverListener.AcceptTcpClient();
                _clientIdSocket["1"] = _serverSocket1;
            });
            Task.WaitAll(t1, t2);
            _notificationHandlers = new Dictionary<string, INotificationHandler>();
            _queueS = new Queue();
            _queueS.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            var whiteBoard = new FakeNotificationHandler();
            _notificationHandlers[Modules.WhiteBoard] = whiteBoard;
            _sendSocketListenerServer = new SendSocketListenerServer(_queueS, _clientIdSocket, _notificationHandlers);
            _sendSocketListenerServer.Start();
            _queueR1 = new Queue();
            _queueR1.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _receiveSocketListener1 = new ReceiveSocketListener(_queueR1, _clientSocket1);
            _receiveSocketListener1.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _serverSocket1.Close();
            _queueR1.Close();
            _queueS.Close();
            _receiveSocketListener1.Stop();
            _sendSocketListenerServer.Stop();
            _clientSocket1.Close();
        }

        private IQueue _queueS;
        private IQueue _queueR1, _queueR2;
        private Machine _server;
        private SendSocketListenerServer _sendSocketListenerServer;
        private ReceiveSocketListener _receiveSocketListener1, _receiveSocketListener2;
        private TcpClient _serverSocket1, _serverSocket2, _clientSocket1, _clientSocket2;
        private Dictionary<string, TcpClient> _clientIdSocket;
        private int _port;
        private IPAddress _ip;
        private TcpListener _serverListener;

        private Dictionary<string, INotificationHandler> _notificationHandlers;

        [Test]
        public void BroadCastServerSendTesting()
        {
            _queueR2 = new Queue();
            _queueR2.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);

            var t1 = Task.Run(() =>
            {
                _clientSocket2 = new TcpClient();
                _clientSocket2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                _clientSocket2.Connect(_ip, _port);
            });
            var t2 = Task.Run(() =>
            {
                _serverSocket2 = _serverListener.AcceptTcpClient();
                _clientIdSocket["2"] = _serverSocket2;
            });
            Task.WaitAll(t1, t2);
            _receiveSocketListener2 = new ReceiveSocketListener(_queueR2, _clientSocket2);
            _receiveSocketListener2.Start();
            var whiteBoardData = "hello ";
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};

            _queueS.Enqueue(whiteBoardPacket);

            while (_queueR1.IsEmpty())
            {
            }

            var packet1 = _queueR1.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet1.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet1.ModuleIdentifier);
            });

            while (_queueR2.IsEmpty())
            {
            }

            var packet2 = _queueR2.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet2.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet2.ModuleIdentifier);
            });
            _serverSocket2.Close();
            _queueR2.Close();
            _receiveSocketListener2.Stop();
            _clientSocket2.Close();
        }

        [Test]
        public void SinglePacketServerSendPrivateTesting()
        {
            var whiteBoardData = "hello ";
            var whiteBoardPacket = new Packet
                {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData, Destination = "1"};
            _queueS.Enqueue(whiteBoardPacket);

            while (_queueR1.IsEmpty())
            {
            }

            var packet = _queueR1.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
            });
        }

        [Test]
        public void BigPacketServerSendTesting()
        {
            var whiteBoardData = NetworkingGlobals.GetRandomString(1500);
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);

            while (_queueR1.IsEmpty())
            {
            }

            var packet = _queueR1.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            });
        }

        [Test]
        public void ClientConnectionLostAfterJoiningServerTesting()
        {
            _clientSocket1.Close();
            _clientSocket1.Dispose();
            var whiteBoardData = "hello ";
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);
            var whiteBoardHandler = (FakeNotificationHandler) _notificationHandlers[Modules.WhiteBoard];
            whiteBoardHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnClientLeft, whiteBoardHandler.Event);
        }
    }
}