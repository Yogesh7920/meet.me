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
        [OneTimeSetUp]
        public void Init_StartServerAndClients_ServerIsNotifiedAboutClients()
        {
            // reset all expando objects of handlers
            _server.Reset();
            _clientA.Reset();
            _clientB.Reset();

            // subscribe modules to server
            _server.Subscribe();
            // start server and store ip and port
            var address = _server.Communicator.Start().Split(":");
            _serverIp = address[0];
            _serverPort = address[1];

            // start client A
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            _clientA.Subscribe();
            _clientACommunicator = _clientA.Communicator;
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
        public void Stop_StopServerAndClients()
        {
            _server.Communicator.Stop();
            _clientA.Communicator.Stop();
            _clientB.Communicator.Stop();
        }

        [SetUp]
        public void ResetAllHandlers()
        {
            _server.Reset();
            _clientA.Reset();
            _clientB.Reset();
        }

        private readonly ISerializer _serializer = new Serializer();
        private string RandomMessage => GetRandomString();
        private string _serverIp, _serverPort;
        private readonly FakeServer _server = new();
        private readonly FakeClientA _clientA = new();
        private readonly FakeClientB _clientB = new();
        private ICommunicator _clientACommunicator;

        [Test]
        public void RemoveClient_ClientLeavesRoom_ServerIsNotifiedAndCannotSendMessage()
        {
            // A module asks the server communicator to remove the client.
            _server.Communicator.RemoveClient(_clientA.Id);
            // All threads on client are stopped.
            _clientA.Communicator.Stop();
            var moduleId = Modules.WhiteBoard;
            var message = RandomMessage;
            var expectedMessage = "Client does not exist in the room!";

            // When server tries to send a message to the same client, error must be thrown.
            Assert.That(() => _server.Communicator.Send(message, moduleId, _clientA.Id),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));

            // Reset client A back to original state.
            _clientA.Communicator = NewClientCommunicator;
            Assert.AreEqual("1", _clientA.Communicator.Start(_serverIp, _serverPort));
            _clientA.Subscribe();
            _server.Communicator.AddClient(_clientA.Id, _server.WbHandler.Data);
        }

        [Test]
        [Description("This test is useful to check if the sockets are disposed and the threads" +
                     "are stopped correctly. If the sockets aren't disposed, the client might not" +
                     "be able to connect again. If the threads are not stopped, this test will timeout.")]
        public void Start_ClientRejoinsCall_ClientShouldReceiveMessages()
        {
            // Simulates a client leaving the room.
            _server.Communicator.RemoveClient(_clientA.Id);
            _clientA.Communicator.Stop();

            // Client re-joining routine begins.
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

            var message = RandomMessage;
            _server.Communicator.Send(message, Modules.WhiteBoard, _clientA.Id);
            _clientA.WbHandler.Wait();
            Assert.AreEqual(OnDataReceived, _clientA.WbHandler.Event);
        }

        [Test]
        public void Send_UnregisteredModuleMustThrow_RegisteredModuleMustNotThrow()
        {
            var message = RandomMessage;
            // Networking module does not exist, so must throw an error.
            var expectedMessage = "Key Error: Packet holds invalid module identifier";

            Assert.That(() => _clientA.Communicator.Send(Modules.Networking, message),
                Throws.TypeOf<Exception>().With.Message.EqualTo(expectedMessage));

            // Whiteboard module exists, so shouldn't throw an error
            Assert.DoesNotThrow(() => _clientA.Communicator.Send(message, Modules.WhiteBoard));
        }

        [Test]
        public void Send_ClientSendsToServer_OnlySendingModuleNotifiedOnServer()
        {
            // Send message as whiteboard module.
            var moduleId = Modules.WhiteBoard;
            var message = RandomMessage;

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
        public void Send_ServerBroadCastsMessage_OnlySendingModuleOnAllClientsNotified()
        {
            var moduleId = Modules.WhiteBoard;
            var message = RandomMessage;

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
        public void Send_ServerSendsPrivateMessage_OnlySendingModuleOnSingleClientNotified()
        {
            var moduleId = Modules.WhiteBoard;
            var message = RandomMessage;

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
            var fakeChat = FakeChat.GetFakeChat();
            // Serializing and deserializing should give the same object.
            var xml = _serializer.Serialize(fakeChat);
            var deserializedChat = _serializer.Deserialize<FakeChat>(xml);
            Assert.AreEqual(fakeChat.ToString(), deserializedChat.ToString());
        }

        [Test]
        public void ObjectType_ShouldReturnCorrectObjectType()
        {
            var fakeChat = FakeChat.GetFakeChat();
            // Serializer should return the correct object type.
            var xml = _serializer.Serialize(fakeChat);
            var objectType = _serializer.GetObjectType(xml, "Testing.Networking");
            Assert.AreEqual(typeof(FakeChat).ToString(), objectType);
        }

        [Test]
        public void Send_SerializeAndSend_SenderShouldReceiveSameMessage()
        {
            // message sent by whiteboard module.
            var moduleId = Modules.WhiteBoard;
            var fakeChat = FakeChat.GetFakeChat();
            var message = _serializer.Serialize(fakeChat);
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
        public void Send_ClientSendsLargeMessage_ServerReceivesLargeMessage()
        {
            var moduleId = Modules.WhiteBoard;
            // This message length is greater than the threshold.
            var message = GetRandomString(2000);

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