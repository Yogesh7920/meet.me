using Networking;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testing.Networking.SocketManagement
{
    [TestFixture]
    public class ReceiveSocketListenerTesting
    {
        private IQueue _queue;
        private Machine _server;
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
        public void StartReceiveSocketListener()
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
            _queue = new Queue();
            _queue.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _receiveSocketListener = new ReceiveSocketListener(_queue, _serverSocket);
            _receiveSocketListener.Start();
        }

        [Test]
        public void SinglePacketReceiveTesting()
        {
            string whiteBoardData = "hello ";
            Packet whiteBoardPacket = new Packet { ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData };
            string msg1 = GetMessage(whiteBoardPacket);
            var stream = _clientSocket.GetStream();
            stream.Write(Encoding.ASCII.GetBytes(msg1), 0, msg1.Length);
            stream.Flush();
            while (_queue.IsEmpty()) { }
            Packet packet = _queue.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
            });
        }
        [Test]
        public void BigPacketReceiveTesting()
        {
            string whiteBoardData = NetworkingGlobals.GetRandomString(4000);
            Packet whiteBoardPacket = new Packet { ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData };
            string message = GetMessage(whiteBoardPacket);
            string msg1 = message;
            var stream = _clientSocket.GetStream();
            stream.Write(Encoding.ASCII.GetBytes(msg1), 0, msg1.Length);
            stream.Flush();

            while (_queue.IsEmpty()) { }
            Packet packet = _queue.Dequeue();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(whiteBoardPacket.ModuleIdentifier, packet.ModuleIdentifier);
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            });
        }

        [Test]
        public void MultiplePacketReceiveTesting()
        {
            for (int i = 1; i <= 10; i++)
            {
                string whiteBoardData = "packet" + i.ToString();
                Packet whiteBoardPacket = new Packet { ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData };
                string msg = GetMessage(whiteBoardPacket);
                var stream = _clientSocket.GetStream();
                stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                stream.Flush();
            }


            Thread.Sleep(100);
            for (int i = 1; i <= 10; i++)
            {
                string whiteBoardData = "packet" + i.ToString();
                Packet packet = _queue.Dequeue();
                Assert.AreEqual(whiteBoardData, packet.SerializedData);
            }
        }
        [TearDown]
        public void TearDown()
        {
            _clientSocket.Close();
            _receiveSocketListener.Stop();
            _serverSocket.Close();
        }
    }
}