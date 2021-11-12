using System;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using Networking;
using NUnit.Framework;

namespace Testing.Networking
{
    [TestFixture]
    public class ModuleTesting
    {
        private ISerializer _serializer = new Serializer();
        private string RandomMessage => NetworkingGlobals.GetRandomString();
        private string _serverIp, _serverPort;

        [OneTimeSetUp]
        public void StartServerAndClientJoin_ServerShouldBeNotified()
        {
            FakeServer.SsHandler.Reset();
            FakeServer.WbHandler.Reset();
            FakeMachineA.SsHandler.Reset();
            FakeMachineA.WbHandler.Reset();
            FakeMachineB.SsHandler.Reset();
            FakeMachineB.WbHandler.Reset();
            FakeServer.Communicator = NetworkingGlobals.NewServerCommunicator;
            FakeServer.Communicator.Subscribe(Modules.WhiteBoard, FakeServer.WbHandler, Priorities.WhiteBoard);
            FakeServer.Communicator.Subscribe(Modules.ScreenShare, FakeServer.SsHandler, Priorities.ScreenShare);
            string[] address = FakeServer.Communicator.Start().Split(":");
            _serverIp = address[0];
            _serverPort = address[1];

            FakeMachineA.Communicator = NetworkingGlobals.NewClientCommunicator;
            FakeMachineA.Communicator.Subscribe(Modules.WhiteBoard, FakeMachineA.WbHandler, Priorities.WhiteBoard);
            FakeMachineA.Communicator.Subscribe(Modules.ScreenShare, FakeMachineA.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", FakeMachineA.Communicator.Start(_serverIp, _serverPort));
            Assert.AreEqual(FakeServer.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(FakeServer.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            FakeServer.Communicator.AddClient(FakeMachineA.ClientID, FakeServer.WbHandler.ReceivedData.Data);
            
            FakeMachineB.Communicator = NetworkingGlobals.NewClientCommunicator;
            FakeMachineB.Communicator.Subscribe(Modules.WhiteBoard, FakeMachineB.WbHandler, Priorities.WhiteBoard);
            FakeMachineB.Communicator.Subscribe(Modules.ScreenShare, FakeMachineB.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", FakeMachineB.Communicator.Start(_serverIp, _serverPort));
            Assert.AreEqual(FakeServer.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(FakeServer.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            FakeServer.Communicator.AddClient(FakeMachineB.ClientID, FakeServer.WbHandler.ReceivedData.Data);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            FakeServer.Communicator.Stop();
            FakeMachineA.Communicator.Stop();
            FakeMachineB.Communicator.Stop();
        }

        [SetUp]
        public void Setup()
        {
            FakeServer.SsHandler.Reset();
            FakeServer.WbHandler.Reset();
            FakeMachineA.SsHandler.Reset();
            FakeMachineA.WbHandler.Reset();
            FakeMachineB.SsHandler.Reset();
            FakeMachineB.WbHandler.Reset();
        }

        [Test]
        public void Subscribe_ModuleShouldBeRegisteredInQueue_DoesNotThrow()
        {
            string message = RandomMessage;
            string expectedMessage = "Key Error: Packet holds invalid module identifier";
            Assert.That(() => FakeMachineA.Communicator.Send(Modules.Networking, message),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));
            Assert.DoesNotThrow(() => FakeMachineA.Communicator.Send(message, Modules.WhiteBoard));
        }

        [Test]
        public void ClientSend_ConcernedModuleOnServerShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            Assert.DoesNotThrow(() => FakeMachineA.Communicator.Send(message, moduleId));
            Thread.Sleep(100);
            Assert.AreNotEqual(NotificationEvents.OnDataReceived, FakeServer.SsHandler.ReceivedData.Event);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeServer.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, FakeServer.WbHandler.ReceivedData.Data);
        }

        [Test]
        public void ServerSend_BroadCastMessage_OnlyConcernedModulesOnAllClientsShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            Assert.DoesNotThrow(() => FakeServer.Communicator.Send(message, moduleId));
            Thread.Sleep(100);
            Assert.AreEqual(FakeMachineA.SsHandler.ReceivedData.Event, null);
            Assert.AreNotEqual(NotificationEvents.OnDataReceived, FakeMachineA.SsHandler.ReceivedData.Event);
        }

        [Test]
        public void ServerSend_PrivateMessage_OnlyConcernedClientShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            Assert.DoesNotThrow(() => FakeServer.Communicator.Send(message, moduleId, FakeMachineA.ClientID));
            Thread.Sleep(100);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeMachineA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(null, FakeMachineA.SsHandler.ReceivedData.Event);
            Assert.AreEqual(null, FakeMachineB.WbHandler.ReceivedData.Event);
            Assert.AreEqual(null, FakeMachineB.SsHandler.ReceivedData.Event);
        }

        [Test]
        public void Serialize_SerializerShouldReturnValidXmlString()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            string xml = _serializer.Serialize(fakeChat);
            FakeChat deserializedChat = _serializer.Deserialize<FakeChat>(xml);
            Assert.AreEqual(fakeChat.ToString(), deserializedChat.ToString());
        }

        [Test]
        public void SerializerObjectType_ShouldReturnCorrectObjectType()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            string xml = _serializer.Serialize(fakeChat);
            string objectType = _serializer.GetObjectType(xml, "Testing.Networking");
            Assert.AreEqual(objectType, typeof(FakeChat).ToString());
        }
        
        [Test]
        public void Send_SerializeAndSend_SenderShouldReceiveSameMessage()
        {
            string moduleId = Modules.WhiteBoard;
            FakeChat fakeChat = FakeChat.GetFakeChat();
            string message = _serializer.Serialize(fakeChat);
            Assert.DoesNotThrow(() => FakeMachineA.Communicator.Send(message, moduleId));
            Thread.Sleep(100);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeServer.WbHandler.ReceivedData.Event);
            FakeChat serverMessage = _serializer.Deserialize<FakeChat>(FakeServer.WbHandler.ReceivedData.Data);
            Assert.AreEqual(serverMessage.ToString(), fakeChat.ToString());
            Assert.DoesNotThrow(() => FakeServer.Communicator.Send(
                _serializer.Serialize(serverMessage), 
                moduleId));
            Thread.Sleep(100);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeMachineA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeMachineB.WbHandler.ReceivedData.Event);
        }

        [Test, Order(1)]
        public void ClientLeft_ServerShouldBeNotified_ClientShouldNotReceiveMessage()
        {
            FakeServer.Communicator.RemoveClient(FakeMachineA.ClientID);
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            string expectedMessage = "Client does not exist in the room!";
            Assert.That(() => FakeServer.Communicator.Send(message, moduleId, FakeMachineA.ClientID),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));
            FakeMachineA.Communicator.Stop();
        }

        [Test, Order(2)]
        public void ClientRejoin_ServerShouldBeNotified_ClientShouldReceiveMessage()
        {
            FakeMachineA.Communicator = NetworkingGlobals.NewClientCommunicator;
            FakeMachineA.Communicator.Subscribe(Modules.WhiteBoard, FakeMachineA.WbHandler, Priorities.WhiteBoard);
            FakeMachineA.Communicator.Subscribe(Modules.ScreenShare, FakeMachineA.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", FakeMachineA.Communicator.Start(_serverIp, _serverPort));
            Assert.AreEqual(FakeServer.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(FakeServer.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Console.WriteLine(FakeServer.WbHandler.ReceivedData.Data);
            FakeServer.Communicator.AddClient(FakeMachineA.ClientID, FakeServer.WbHandler.ReceivedData.Data);
        }

        [Test]
        public void ClientSend_LargeMessage_ShouldNotAlterFunctionality()
        {
            string moduleId = Modules.WhiteBoard;
            string message = NetworkingGlobals.GetRandomString(2000);
            Assert.DoesNotThrow(() => FakeMachineA.Communicator.Send(message, moduleId));
            Thread.Sleep(100);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeServer.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, FakeServer.WbHandler.ReceivedData.Data);
            Assert.DoesNotThrow(() => FakeServer.Communicator.Send(
                FakeServer.WbHandler.ReceivedData.Data, 
                moduleId));
            Thread.Sleep(100);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeMachineA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeMachineB.WbHandler.ReceivedData.Event);
        }
    }
}