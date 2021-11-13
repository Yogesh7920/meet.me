using System;
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
        private FakeServer _server = new FakeServer();
        private FakeClientA _clientA = new FakeClientA();
        private FakeClientB _clientB = new FakeClientB();

        [OneTimeSetUp]
        public void StartServerAndClientJoin_ServerShouldBeNotified()
        {
            // reset all expando objects of handlers
            _server.Reset();
            _clientA.Reset();
            _clientB.Reset();

            // subscribe modules to server
            _server.Communicator.Subscribe(Modules.WhiteBoard, _server.WbHandler, Priorities.WhiteBoard);
            _server.Communicator.Subscribe(Modules.ScreenShare, _server.SsHandler, Priorities.ScreenShare);
            // start server and store ip and port
            string[] address = _server.Communicator.Start().Split(":");
            _serverIp = address[0];
            _serverPort = address[1];
            
            // start client A
            _clientA.Communicator.Subscribe(Modules.WhiteBoard, _clientA.WbHandler, Priorities.WhiteBoard);
            _clientA.Communicator.Subscribe(Modules.ScreenShare, _clientA.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            Assert.AreEqual(_server.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(_server.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            // add client A to server
            _server.Communicator.AddClient(_clientA.Id, _server.WbHandler.ReceivedData.Data);
            
            // start clientB
            _clientB.Communicator.Subscribe(Modules.WhiteBoard, _clientB.WbHandler, Priorities.WhiteBoard);
            _clientB.Communicator.Subscribe(Modules.ScreenShare, _clientB.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", _clientB.Communicator.Start(_serverIp, _serverPort));
            Assert.AreEqual(_server.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(_server.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            // add clientB to server
            _server.Communicator.AddClient(_clientB.Id, _server.WbHandler.ReceivedData.Data);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _server.Communicator.Stop();
            _clientA.Communicator.Stop();
            _clientB.Communicator.Stop();
        }

        [SetUp]
        public void Setup()
        {
            _server.Reset();
            _clientA.Reset();
            _clientB.Reset();
        }
        
        [Test, Order(1)]
        public void ClientLeft_ServerShouldBeNotified_ClientShouldNotReceiveMessage()
        {
            // Module that realises the client has left calls the RemoveClient method on the server.
            _server.Communicator.RemoveClient(_clientA.Id);
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            string expectedMessage = "Client does not exist in the room!";
            // When server tries to send a message to the same client, error must be thrown.
            Assert.That(() => _server.Communicator.Send(message, moduleId, _clientA.Id),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));
            // Cleanup all threads on the client.
            _clientA.Communicator.Stop();
        }
        
        [Test, Order(2)]
        public void ClientRejoin_ServerShouldBeNotified_ClientShouldReceiveMessage()
        {
            // Client A left in the previous test and rejoins now
            _clientA.Communicator = NetworkingGlobals.NewClientCommunicator;
            // Client A subscribes all its modules to send and receive data
            _clientA.Communicator.Subscribe(Modules.WhiteBoard, _clientA.WbHandler, Priorities.WhiteBoard);
            _clientA.Communicator.Subscribe(Modules.ScreenShare, _clientA.SsHandler, Priorities.ScreenShare);
            // Start client, must return a success message.
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            _server.WbHandler.Wait();
            // All modules on server will be notified that a client has joined
            Assert.AreEqual(_server.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(_server.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            // A module on the server will add the client to the networking module
            _server.Communicator.AddClient(_clientA.Id, _server.WbHandler.ReceivedData.Data);
        }

        [Test]
        public void Subscribe_ModuleShouldBeRegisteredInQueue_DoesNotThrow()
        {
            string message = RandomMessage;
            // Networking module does not exist, so must throw an error.
            string expectedMessage = "Key Error: Packet holds invalid module identifier";
            Assert.That(() => _clientA.Communicator.Send(Modules.Networking, message),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));
            // Whiteboard module exists, so shouldn't throw an error
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, Modules.WhiteBoard));
        }

        [Test]
        public void ClientSend_ConcernedModuleOnServerShouldBeNotified()
        {
            // Send message as whiteboard module.
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            // Send should be successful
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, moduleId));
            _server.WbHandler.Wait();
            // Since whiteboard module sent the message, screenshare shouldn't receive it.
            Assert.AreNotEqual(NotificationEvents.OnDataReceived, _server.SsHandler.ReceivedData.Event);
            // Whiteboard should receive the same message
            Assert.AreEqual(NotificationEvents.OnDataReceived, _server.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _server.WbHandler.ReceivedData.Data);
        }

        [Test]
        public void ServerSend_BroadCastMessage_OnlyConcernedModulesOnAllClientsShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            // Server send shouldn't fail
            Assert.DoesNotThrow(() => _server.Communicator.Send(message, moduleId));
            _clientA.WbHandler.Wait();
            // Only whiteboard module on client A should receive it.
            Assert.AreEqual(_clientA.SsHandler.ReceivedData.Event, null);
            Assert.AreEqual(_clientA.WbHandler.ReceivedData.Event, NotificationEvents.OnDataReceived);
            _clientB.WbHandler.Wait();
            // Only whiteboard module on client B should receive it.
            Assert.AreEqual(_clientB.SsHandler.ReceivedData.Event, null);
            Assert.AreEqual(_clientB.WbHandler.ReceivedData.Event, NotificationEvents.OnDataReceived);
        }

        [Test]
        public void ServerSend_PrivateMessage_OnlyConcernedClientShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            // Server send shouldn't fail
            Assert.DoesNotThrow(() => _server.Communicator.Send(message, moduleId, _clientA.Id));
            _clientA.WbHandler.Wait();
            // Only whiteboard module on client A should receive it.
            Assert.AreEqual(NotificationEvents.OnDataReceived, _clientA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(null, _clientA.SsHandler.ReceivedData.Event);
            // None of the modules on client B should receive it.
            Assert.AreEqual(null, _clientB.WbHandler.ReceivedData.Event);
            Assert.AreEqual(null, _clientB.SsHandler.ReceivedData.Event);
        }

        [Test]
        public void Serialize_SerializerShouldReturnValidXmlString()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            // Serializing and deserializing should give the same object.
            string xml = _serializer.Serialize(fakeChat);
            FakeChat deserializedChat = _serializer.Deserialize<FakeChat>(xml);
            Assert.AreEqual(deserializedChat.ToString(), fakeChat.ToString());
        }

        [Test]
        public void SerializerObjectType_ShouldReturnCorrectObjectType()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            // Serializer should return the correct object type.
            string xml = _serializer.Serialize(fakeChat);
            string objectType = _serializer.GetObjectType(xml, "Testing.Networking");
            Assert.AreEqual(objectType, typeof(FakeChat).ToString());
        }
        
        [Test]
        public void Send_SerializeAndSend_SenderShouldReceiveSameMessage()
        {
            // message sent by whiteboard module.
            string moduleId = Modules.WhiteBoard;
            FakeChat fakeChat = FakeChat.GetFakeChat();
            string message = _serializer.Serialize(fakeChat);
            // client A serializes and sends the message
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, moduleId));
            _server.WbHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, _server.WbHandler.ReceivedData.Event);
            // whiteboard module on server receives the same object
            FakeChat serverMessage = _serializer.Deserialize<FakeChat>(_server.WbHandler.ReceivedData.Data);
            Assert.AreEqual(fakeChat.ToString(), serverMessage.ToString());
            // server broadcasts the received object.
            Assert.DoesNotThrow(() => _server.Communicator.Send(
                _serializer.Serialize(serverMessage), 
                moduleId));
            _clientA.WbHandler.Wait();
            // whiteboard module on client A and B receives the same message that was sent by client A.
            Assert.AreEqual(NotificationEvents.OnDataReceived, _clientA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _clientA.WbHandler.ReceivedData.Data);
            _clientB.WbHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, _clientB.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _clientB.WbHandler.ReceivedData.Data);
        }

        [Test]
        public void ClientSend_LargeMessage_ShouldNotAlterFunctionality()
        {
            string moduleId = Modules.WhiteBoard;
            // This message length is greater than the threshold.
            string message = NetworkingGlobals.GetRandomString(2000);
            // Client A sends the large message.
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, moduleId));
            _server.WbHandler.Wait();
            // The whiteboard module on the server receives the same message.
            Assert.AreEqual(NotificationEvents.OnDataReceived, _server.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _server.WbHandler.ReceivedData.Data);
            // Server broadcasts the message
            Assert.DoesNotThrow(() => _server.Communicator.Send(
                _server.WbHandler.ReceivedData.Data, 
                moduleId));
            // Both the clients should receive the same message.
            _clientA.WbHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, _clientA.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _clientA.WbHandler.ReceivedData.Data);
            _clientB.WbHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, _clientB.WbHandler.ReceivedData.Event);
            Assert.AreEqual(message, _clientB.WbHandler.ReceivedData.Data);
        }
    }
}