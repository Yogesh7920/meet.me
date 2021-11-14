using System;
using Networking;
using NUnit.Framework;
using static Testing.Networking.NetworkingGlobals;
using static Testing.Networking.NotificationEvents;

namespace Testing.Networking
{
    [TestFixture]
    public class ModuleTesting
    {
        private readonly ISerializer _serializer = new Serializer();
        private string RandomMessage => GetRandomString();
        private string _serverIp, _serverPort;
        private readonly FakeServer _server = new();
        private readonly FakeClientA _clientA = new();
        private readonly FakeClientB _clientB = new();

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // reset all expando objects of handlers
            _server.Reset();
            _clientA.Reset();
            _clientB.Reset();
        }
        
        [Test, Order(0)]
        public void StartServerAndClientJoin_ServerShouldBeNotified()
        {
            // subscribe modules to server
            _server.Subscribe();
            // start server and store ip and port
            string[] address = _server.Communicator.Start().Split(":");
            _serverIp = address[0];
            _serverPort = address[1];
            
            // start client A
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            _clientA.Subscribe();
            Assert.AreEqual(OnClientJoined, _server.WbHandler.Event);
            Assert.AreEqual(OnClientJoined, _server.SsHandler.Event);
            // add client A to server
            _server.Communicator.AddClient(_clientA.Id, _server.WbHandler.Data);
            
            // start clientB
            Assert.AreEqual("1", _clientB.Communicator.Start(_serverIp, _serverPort));
            _clientB.Subscribe();
            Assert.AreEqual(OnClientJoined, _server.WbHandler.Event);
            Assert.AreEqual(OnClientJoined, _server.SsHandler.Event);
            // add clientB to server
            _server.Communicator.AddClient(_clientB.Id, _server.WbHandler.Data);
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
            _clientA.Communicator = NewClientCommunicator;
            // Start client, must return a success message.
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            // Client A subscribes all its modules to send and receive data
            _clientA.Subscribe();
            
            _server.WbHandler.Wait();
            // All modules on server will be notified that a client has joined
            Assert.AreEqual(OnClientJoined, _server.WbHandler.Event);
            Assert.AreEqual(OnClientJoined, _server.SsHandler.Event);
            
            // A module on the server will add the client to the networking module
            _server.Communicator.AddClient(_clientA.Id, _server.WbHandler.Data);
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
            // Since whiteboard module sent the message, screen-share shouldn't receive it.
            Assert.AreEqual(null, _server.SsHandler.Event);
            // Whiteboard should receive the same message
            Assert.AreEqual(OnDataReceived, _server.WbHandler.Event);
            Assert.AreEqual(message, _server.WbHandler.Data);
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
            Assert.AreEqual(null, _clientA.SsHandler.Event);
            Assert.AreEqual(OnDataReceived, _clientA.WbHandler.Event);
            
            _clientB.WbHandler.Wait();
            // Only whiteboard module on client B should receive it.
            Assert.AreEqual(null, _clientB.SsHandler.Event);
            Assert.AreEqual(OnDataReceived, _clientB.WbHandler.Event);
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
            Assert.AreEqual(OnDataReceived, _clientA.WbHandler.Event);
            Assert.AreEqual(null, _clientA.SsHandler.Event);
            // None of the modules on client B should receive it.
            Assert.AreEqual(null, _clientB.WbHandler.Event);
            Assert.AreEqual(null, _clientB.SsHandler.Event);
        }

        [Test]
        public void Serialize_SerializerShouldReturnValidXmlString()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            // Serializing and deserializing should give the same object.
            string xml = _serializer.Serialize(fakeChat);
            FakeChat deserializedChat = _serializer.Deserialize<FakeChat>(xml);
            Assert.AreEqual(fakeChat.ToString(), deserializedChat.ToString());
        }

        [Test]
        public void SerializerObjectType_ShouldReturnCorrectObjectType()
        {
            FakeChat fakeChat = FakeChat.GetFakeChat();
            // Serializer should return the correct object type.
            string xml = _serializer.Serialize(fakeChat);
            string objectType = _serializer.GetObjectType(xml, "Testing.Networking");
            Assert.AreEqual(typeof(FakeChat).ToString(), objectType);
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
            Assert.AreEqual(OnDataReceived, _server.WbHandler.Event);
            
            // whiteboard module on server receives the same object
            FakeChat serverMessage = _serializer.Deserialize<FakeChat>(_server.WbHandler.Data);
            Assert.AreEqual(fakeChat.ToString(), serverMessage.ToString());
            
            // server broadcasts the received object.
            Assert.DoesNotThrow(() => _server.Communicator.Send(
                _serializer.Serialize(serverMessage), 
                moduleId));
            
            _clientA.WbHandler.Wait();
            // whiteboard module on client A and B receives the same message that was sent by client A.
            Assert.AreEqual(OnDataReceived, _clientA.WbHandler.Event);
            Assert.AreEqual(message, _clientA.WbHandler.Data);
            
            _clientB.WbHandler.Wait();
            Assert.AreEqual(OnDataReceived, _clientB.WbHandler.Event);
            Assert.AreEqual(message, _clientB.WbHandler.Data);
        }

        [Test]
        public void ClientSend_LargeMessage_ShouldNotAlterFunctionality()
        {
            string moduleId = Modules.WhiteBoard;
            // This message length is greater than the threshold.
            string message = GetRandomString(2000);
            
            // Client A sends the large message.
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, moduleId));
            
            _server.WbHandler.Wait();
            // The whiteboard module on the server receives the same message.
            Assert.AreEqual(OnDataReceived, _server.WbHandler.Event);
            Assert.AreEqual(message, _server.WbHandler.Data);
            
            // Server broadcasts the message
            Assert.DoesNotThrow(() => _server.Communicator.Send(
                _server.WbHandler.Data, 
                moduleId));
            
            // Both the clients should receive the same message.
            _clientA.WbHandler.Wait();
            Assert.AreEqual(OnDataReceived, _clientA.WbHandler.Event);
            Assert.AreEqual(message, _clientA.WbHandler.Data);
            
            _clientB.WbHandler.Wait();
            Assert.AreEqual(OnDataReceived, _clientB.WbHandler.Event);
            Assert.AreEqual(message, _clientB.WbHandler.Data);
        }
    }
}