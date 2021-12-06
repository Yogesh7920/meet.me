/// <author>Tausif Iqbal</author>
/// <created>01/11/2021</created>
/// <summary>
/// This file covers the unit tests for for the class SendSocketListenerClient.
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class SendSocketListenerClientTesting
    {
        [SetUp]
        public void StartSendSocketListenerClient()
        {
            _server = new FakeServer();
            var address = _server.Communicator.Start().Split(":");
            var port = int.Parse(address[1]);
            var ip = IPAddress.Parse(address[0]);
            _server.Communicator.Stop();
            var serverSocket = new TcpListener(ip, port);
            serverSocket.Start();
            _clientSocket = new TcpClient();
            _clientSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            var t1 = Task.Run(() => { _clientSocket.Connect(ip, port); });
            var t2 = Task.Run(() => { _serverSocket = serverSocket.AcceptTcpClient(); });
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

        [TearDown]
        public void TearDown()
        {
            _clientSocket.Close();
            _queueR.Close();
            _queueS.Close();
            _sendSocketListenerClient.Stop();
            _receiveSocketListener.Stop();
            _serverSocket.Close();
        }

        private IQueue _queueS;
        private IQueue _queueR;
        private Machine _server;
        private SendSocketListenerClient _sendSocketListenerClient;
        private ReceiveSocketListener _receiveSocketListener;
        private TcpClient _serverSocket;
        private TcpClient _clientSocket;

        [Test]
        public void SinglePacketClientSendTesting()
        {
            var whiteBoardData = "hello ";
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);

            while (_queueR.IsEmpty())
            {
            }

            var packet = _queueR.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
            });
        }

        [Test]
        public void BigPacketClientSendTesting()
        {
            var whiteBoardData = NetworkingGlobals.GetRandomString(4000);
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            _queueS.Enqueue(whiteBoardPacket);

            while (_queueR.IsEmpty())
            {
            }

            var packet = _queueR.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            });
        }

        [Test]
        public void MultiplePacketClientSendTesting()
        {
            for (var i = 1; i <= 10; i++)
            {
                var whiteBoardData = "packet" + i;
                var whiteBoardPacket = new Packet
                    {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
                _queueS.Enqueue(whiteBoardPacket);
            }


            while (_queueR.Size() != 10) Thread.Sleep(10);
            for (var i = 1; i <= 10; i++)
            {
                var whiteBoardData = "packet" + i;
                var packet = _queueR.Dequeue();
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            }
        }
    }
}