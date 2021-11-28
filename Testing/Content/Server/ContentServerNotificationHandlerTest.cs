/// <author>Sameer Dhiman</author>
/// <created>7/11/2021</created>
/// <summary>
///     This file contains tests for ContentServerNotificationHandler
/// </summary>
using Content;
using Networking;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Testing.Content
{
    public class ContentServerNotificationHandlerTests
    {
        private Utils utils;
        private ContentServer contentServer;
        private INotificationHandler notificationHandler;
        private ISerializer serializer;
        private FakeContentListener listener;
        private FakeCommunicator communicator;
        private int sleeptime;

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
            MessageData messageData = utils.GenerateNewMessageData("Hello");

            string serializedMessage = serializer.Serialize(messageData);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            ReceiveMessageData notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello", notifiedMessage.Message);
            Assert.AreEqual(messageData.Type, notifiedMessage.Type);
            Assert.AreEqual(messageData.Event, notifiedMessage.Event);
            Assert.AreEqual(messageData.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(messageData.Starred, notifiedMessage.Starred);
            Assert.AreEqual(messageData.ReceiverIds, notifiedMessage.ReceiverIds);

            string sentMessage = communicator.GetSentData();

            MessageData deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

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
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            MessageData file = new MessageData
            {
                Message = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderId = 1,
                ReplyThreadId = -1,
                Event = MessageEvent.NewMessage,
                ReceiverIds = new int[0]
            };

            string serializedMessage = serializer.Serialize(file);

            notificationHandler.OnDataReceived(serializedMessage);

            Thread.Sleep(sleeptime);

            ReceiveMessageData notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Test_File.pdf", notifiedMessage.Message);
            Assert.AreEqual(file.Type, notifiedMessage.Type);
            Assert.AreEqual(file.Event, notifiedMessage.Event);
            Assert.AreEqual(file.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(file.Starred, notifiedMessage.Starred);
            Assert.AreEqual(file.ReceiverIds, notifiedMessage.ReceiverIds);

            string sentMessage = communicator.GetSentData();

            MessageData deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

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
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            string garbageData = " adfasfasfsadf";
            notificationHandler.OnDataReceived(garbageData);

            Thread.Sleep(sleeptime);

            ReceiveMessageData currentMessageToSubscribers = listener.GetOnMessageData();

            Assert.AreEqual(currentMessageToSubscribers, previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }
    }
}