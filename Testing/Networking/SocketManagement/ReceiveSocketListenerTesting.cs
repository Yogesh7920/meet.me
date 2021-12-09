/// <author>Tausif Iqbal</author>
/// <created>01/11/2021</created>
/// <summary>
/// This file covers the unit tests for for the class ReceiveSocketListener.
/// </summary>

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
    public class ReceiveSocketListenerTesting
    {
        [SetUp]
        public void StartReceiveSocketListener()
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
            _queue = new Queue();
            _queue.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _receiveSocketListener = new ReceiveSocketListener(_queue, _serverSocket);
            _receiveSocketListener.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _clientSocket.Close();
            _queue.Close();
            _receiveSocketListener.Stop();
            _serverSocket.Close();
        }

        private IQueue _queue;
        private Machine _server;
        private ReceiveSocketListener _receiveSocketListener;
        private TcpClient _serverSocket;
        private TcpClient _clientSocket;

        [Test]
        public void SinglePacketReceiveTesting()
        {
            const string whiteBoardData = "hello ";
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            var msg1 = Utils.GetMessage(whiteBoardPacket);
            var stream = _clientSocket.GetStream();
            stream.Write(Encoding.ASCII.GetBytes(msg1), 0, msg1.Length);
            stream.Flush();
            while (_queue.IsEmpty())
            {
            }

            var packet = _queue.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
            });
        }

        [Test]
        public void BigPacketReceiveTesting()
        {
            var whiteBoardData = NetworkingGlobals.GetRandomString(4000);
            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            var message = Utils.GetMessage(whiteBoardPacket);
            var stream = _clientSocket.GetStream();
            stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
            stream.Flush();

            while (_queue.IsEmpty())
            {
            }

            var packet = _queue.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            });
        }

        [Test]
        public void MultiplePacketReceiveTesting()
        {
            for (var i = 1; i <= 10; i++)
            {
                var whiteBoardData = "packet" + i;
                var whiteBoardPacket = new Packet
                    {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
                var msg = Utils.GetMessage(whiteBoardPacket);
                _clientSocket.Client.Send(Encoding.ASCII.GetBytes(msg));
            }

            while (_queue.Size() != 10) Thread.Sleep(10);
            for (var i = 1; i <= 10; i++)
            {
                var whiteBoardData = "packet" + i;
                var packet = _queue.Dequeue();
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            }
        }
    }
}