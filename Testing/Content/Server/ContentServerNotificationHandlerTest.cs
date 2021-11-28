/// <author>Sameer Dhiman</author>
/// <created>7/11/2021</created>
/// <summary>
///     This file contains tests for ContentServerNotificationHandler
/// </summary>

using System;
using System.IO;
using System.Threading;
using Content;
using Networking;
using NUnit.Framework;

namespace Testing.Content
{
    public class ContentServerNotificationHandlerTests
    {
        private FakeCommunicator communicator;
        private ContentServer contentServer;
        private FakeContentListener listener;
        private INotificationHandler notificationHandler;
        private ISerializer serializer;
        private int sleeptime;
        private Utils utils;

        [SetUp]
        public void Setup()
        {
            utils = new Utils();
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();
            notificationHandler = new ContentServerNotificationHandler(contentServer);
            serializer = new Serializer();
            listener = new FakeContentListener();
            communicator = new FakeCommunicator();
            contentServer.Communicator = communicator;
            contentServer.SSubscribe(listener);
            sleeptime = 50;
        }

        [Test]
        public void OnDataReceived_ChatDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
            var messageData = utils.GenerateNewMessageData("Hello");

            var serializedMessage = serializer.Serialize(messageData);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello", notifiedMessage.Message);
            Assert.AreEqual(messageData.Type, notifiedMessage.Type);
            Assert.AreEqual(messageData.Event, notifiedMessage.Event);
            Assert.AreEqual(messageData.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(messageData.Starred, notifiedMessage.Starred);
            Assert.AreEqual(messageData.ReceiverIds, notifiedMessage.ReceiverIds);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

            Assert.AreEqual("Hello", deserializesSentMessage.Message);
            Assert.AreEqual(messageData.Type, deserializesSentMessage.Type);
            Assert.AreEqual(messageData.Event, deserializesSentMessage.Event);
            Assert.AreEqual(messageData.SenderId, deserializesSentMessage.SenderId);
            Assert.AreEqual(messageData.Starred, deserializesSentMessage.Starred);
            Assert.AreEqual(messageData.ReceiverIds, deserializesSentMessage.ReceiverIds);
            Assert.IsTrue(communicator.GetIsBroadcast());
        }

        [Test]
        public void OnDataReceived_FileDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var file = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage,
                ReceiverIds = new int[0]
            };

            var serializedMessage = serializer.Serialize(file);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Test_File.pdf", notifiedMessage.Message);
            Assert.AreEqual(file.Type, notifiedMessage.Type);
            Assert.AreEqual(file.Event, notifiedMessage.Event);
            Assert.AreEqual(file.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(file.Starred, notifiedMessage.Starred);
            Assert.AreEqual(file.ReceiverIds, notifiedMessage.ReceiverIds);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

            Assert.AreEqual("Test_File.pdf", deserializesSentMessage.Message);
            Assert.AreEqual(file.Type, deserializesSentMessage.Type);
            Assert.AreEqual(file.Event, deserializesSentMessage.Event);
            Assert.AreEqual(file.SenderId, deserializesSentMessage.SenderId);
            Assert.AreEqual(file.Starred, deserializesSentMessage.Starred);
            Assert.AreEqual(file.ReceiverIds, deserializesSentMessage.ReceiverIds);
            Assert.IsTrue(communicator.GetIsBroadcast());
        }

        [Test]
        public void OnDataReceived_InvalidDataIsReceived_CallReceiveMethodOfContentDatabase()
        {
            var previousMessageToSubsribers = listener.GetOnMessageData();
            var previousMessageToCommunicator = communicator.GetSentData();

            var garbageData = " adfasfasfsadf";
            notificationHandler.OnDataReceived(garbageData);

            Thread.Sleep(sleeptime);

            var currentMessageToSubscribers = listener.GetOnMessageData();

            Assert.AreEqual(currentMessageToSubscribers, previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }
    }
}