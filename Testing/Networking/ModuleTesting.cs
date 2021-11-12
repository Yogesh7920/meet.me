using System;
using System.Threading;
using Networking;
using NUnit.Framework;

namespace Testing.Networking
{

    public static class FakeMachineA
    {
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }
    
    public static class FakeMachineB
    {
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }

    public static class FakeServer
    {
        public static ICommunicator Communicator;
        public static FakeNotificationHandler WbHandler = new(), SsHandler = new();
    }
    
    [TestFixture]
    public class ModuleTesting
    {
        private ICommunicator NewClientCommunicator => CommunicationFactory.GetCommunicator(true, true);
        private ICommunicator NewServerCommunicator => CommunicationFactory.GetCommunicator(false, true);
        private string RandomMessage => NetworkingGlobals.GetRandomString();

        [OneTimeSetUp]
        [Ignore("")]
        public void StartServerAndClientJoin_ServerShouldBeNotified()
        {
            FakeServer.Communicator = NewServerCommunicator;
            FakeServer.Communicator.Subscribe(Modules.WhiteBoard, FakeServer.WbHandler, Priorities.WhiteBoard);
            FakeServer.Communicator.Subscribe(Modules.ScreenShare, FakeServer.SsHandler, Priorities.ScreenShare);
            string[] address = FakeServer.Communicator.Start().Split(":");
            string ip = address[0], port = address[1];
            
            FakeMachineA.Communicator = NewClientCommunicator;
            FakeMachineA.Communicator.Subscribe(Modules.WhiteBoard, FakeMachineA.WbHandler, Priorities.WhiteBoard);
            FakeMachineA.Communicator.Subscribe(Modules.ScreenShare, FakeMachineA.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", FakeMachineA.Communicator.Start(ip, port));
            Assert.AreEqual(FakeServer.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(FakeServer.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            
            FakeMachineB.Communicator = NewClientCommunicator;
            FakeMachineB.Communicator.Subscribe(Modules.WhiteBoard, FakeMachineB.WbHandler, Priorities.WhiteBoard);
            FakeMachineB.Communicator.Subscribe(Modules.ScreenShare, FakeMachineB.SsHandler, Priorities.ScreenShare);
            Assert.AreEqual("1", FakeMachineB.Communicator.Start(ip, port));
            Assert.AreEqual(FakeServer.WbHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
            Assert.AreEqual(FakeServer.SsHandler.ReceivedData.Event, NotificationEvents.OnClientJoined);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            FakeServer.Communicator.Stop();
            FakeMachineA.Communicator.Stop();
            FakeMachineB.Communicator.Stop();
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
        
        // [Test]
        public void ClientSend_ServerShouldBeNotified()
        {
            string moduleId = Modules.WhiteBoard;
            string message = RandomMessage;
            Assert.DoesNotThrow(() => FakeMachineA.Communicator.Send(message, moduleId));
            Thread.Sleep(300);
            Assert.AreEqual(NotificationEvents.OnDataReceived, FakeServer.WbHandler.ReceivedData.Event);
        }
    }
}