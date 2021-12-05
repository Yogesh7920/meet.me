/// <author>Alisetti Sai Vamsi</author>
/// <created>12/11/2021</created>
/// <summary>
/// This file contains the unit tests for ReceiveQueueListener Module.
/// </summary>

using System.Collections.Generic;
using Networking;
using NUnit.Framework;

namespace Testing.Networking.QueueManagement
{
    [TestFixture]
    public class ReceiveQueueListenerTest
    {
        [SetUp]
        public void Setup()
        {
            _queue = new Queue();
            _notificationHandlers = new Dictionary<string, INotificationHandler>();
            _queue.RegisterModule(Modules.WhiteBoard, Priorities.WhiteBoard);
            _queue.RegisterModule(Modules.ScreenShare, Priorities.ScreenShare);
            _queue.RegisterModule(Modules.File, Priorities.File);

            var fakeWhiteBoard = new FakeNotificationHandler();
            var fakeScreenShare = new FakeNotificationHandler();
            var fakeFileShare = new FakeNotificationHandler();

            _notificationHandlers[Modules.WhiteBoard] = fakeWhiteBoard;
            _notificationHandlers[Modules.ScreenShare] = fakeScreenShare;
            _notificationHandlers[Modules.File] = fakeFileShare;

            _receiveQueueListener = new ReceiveQueueListener(_queue, _notificationHandlers);
            _receiveQueueListener.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _queue.Close();
            _receiveQueueListener.Stop();
            _queue = null;
            _receiveQueueListener = null;
            _notificationHandlers = null;
        }

        private IQueue _queue;
        private Dictionary<string, INotificationHandler> _notificationHandlers;
        private ReceiveQueueListener _receiveQueueListener;

        private static string Message => NetworkingGlobals.GetRandomString();

        [Test]
        public void ListenQueue_DequeuingFromQueueAndCallingHandler_ShouldCallAppropriateHandler()
        {
            var whiteBoardData = Message;
            var screenShareData = Message;
            var fileShareData = Message;

            var whiteBoardPacket = new Packet {ModuleIdentifier = Modules.WhiteBoard, SerializedData = whiteBoardData};
            var screenSharePacket = new Packet
                {ModuleIdentifier = Modules.ScreenShare, SerializedData = screenShareData};
            var fileSharePacket = new Packet {ModuleIdentifier = Modules.File, SerializedData = fileShareData};

            _queue.Enqueue(whiteBoardPacket);
            _queue.Enqueue(screenSharePacket);
            _queue.Enqueue(fileSharePacket);

            var whiteBoardHandler = (FakeNotificationHandler) _notificationHandlers[Modules.WhiteBoard];
            var screenShareHandler = (FakeNotificationHandler) _notificationHandlers[Modules.ScreenShare];
            var fileShareHandler = (FakeNotificationHandler) _notificationHandlers[Modules.File];

            screenShareHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, screenShareHandler.Event);
            Assert.AreEqual(screenShareData, screenShareHandler.Data);

            whiteBoardHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, whiteBoardHandler.Event);
            Assert.AreEqual(whiteBoardData, whiteBoardHandler.Data);

            fileShareHandler.Wait();
            Assert.AreEqual(NotificationEvents.OnDataReceived, fileShareHandler.Event);
            Assert.AreEqual(fileShareData, fileShareHandler.Data);
        }
    }
}