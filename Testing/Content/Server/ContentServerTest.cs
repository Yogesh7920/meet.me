/// <author>Sameer Dhiman</author>
/// <created>7/11/2021</created>
/// <summary>
///     This file contains tests for ContentServer
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Content;
using Networking;
using NUnit.Framework;

namespace Testing.Content
{
    public class ContentServerTests
    {
        private FakeCommunicator communicator;
        private ContentServer contentServer;
        private FakeContentListener listener;
        private ISerializer serializer;
        private int sleeptime;
        private Utils utils;

        [SetUp]
        public void Setup()
        {
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            contentServer.Reset();

            utils = new Utils();
            listener = new FakeContentListener();
            contentServer.SSubscribe(listener);
            communicator = new FakeCommunicator();
            contentServer.Communicator = communicator;
            serializer = new Serializer();
            sleeptime = 50;

            var messageData = utils.GenerateNewMessageData("First Message");
            var serializedMessage = serializer.Serialize(messageData);
            contentServer.Receive(serializedMessage);

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

            serializedMessage = serializer.Serialize(file);

            contentServer.Receive(serializedMessage);
        }

        [Test]
        public void SSubscribe_SubsribingToNotification_ShouldBeAbleToGetNotificationOfNewMessages()
        {
            var messageData = utils.GenerateNewMessageData("Hello");

            var serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

            Thread.Sleep(sleeptime);

            var notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello", notifiedMessage.Message);
            Assert.AreEqual(messageData.Type, notifiedMessage.Type);
            Assert.AreEqual(messageData.Event, notifiedMessage.Event);
            Assert.AreEqual(messageData.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(messageData.Starred, notifiedMessage.Starred);
            Assert.AreEqual(messageData.ReceiverIds, notifiedMessage.ReceiverIds);
        }

        [Test]
        public void
            Receive_HandlingNewMessage_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            var messageData = utils.GenerateNewMessageData("Hello");

            var serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

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
        public void
            Receive_HandlingNewFile_ShouldSaveTheNewFileAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
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

            var serializesMessage = serializer.Serialize(file);

            contentServer.Receive(serializesMessage);

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
        public void
            Receive_StarringAMessage_ShouldStarTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            var starMessage = new MessageData
            {
                MessageId = 0,
                ReplyThreadId = 0,
                Event = MessageEvent.Star,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            Thread.Sleep(sleeptime);

            var starredMessage = listener.GetOnMessageData();

            Assert.AreEqual("First Message", starredMessage.Message);
            Assert.AreEqual(MessageType.Chat, starredMessage.Type);
            Assert.AreEqual(MessageEvent.Star, starredMessage.Event);
            Assert.AreEqual(0, starredMessage.MessageId);
            Assert.AreEqual(0, starredMessage.ReplyThreadId);
            Assert.IsTrue(starredMessage.Starred);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

            Assert.AreEqual("First Message", deserializesSentMessage.Message);
            Assert.AreEqual(MessageType.Chat, deserializesSentMessage.Type);
            Assert.AreEqual(MessageEvent.Star, deserializesSentMessage.Event);
            Assert.AreEqual(0, deserializesSentMessage.MessageId);
            Assert.AreEqual(0, deserializesSentMessage.ReplyThreadId);
            Assert.IsTrue(deserializesSentMessage.Starred);
            Assert.IsTrue(communicator.GetIsBroadcast());
        }

        [Test]
        public void
            Receive_UpdatingAMessage_ShouldUpdateTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            var updateMessage = new MessageData
            {
                MessageId = 0,
                ReplyThreadId = 0,
                Event = MessageEvent.Update,
                Type = MessageType.Chat,
                Message = "Hello World!"
            };

            var serializedUpdateMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedUpdateMessage);

            Thread.Sleep(sleeptime);

            var updatedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello World!", updatedMessage.Message);
            Assert.AreEqual(MessageType.Chat, updatedMessage.Type);
            Assert.AreEqual(MessageEvent.Update, updatedMessage.Event);
            Assert.AreEqual(0, updatedMessage.MessageId);
            Assert.AreEqual(0, updatedMessage.ReplyThreadId);

            var sentMessage = communicator.GetSentData();

            var deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

            Assert.AreEqual("Hello World!", deserializesSentMessage.Message);
            Assert.AreEqual(MessageType.Chat, deserializesSentMessage.Type);
            Assert.AreEqual(MessageEvent.Update, deserializesSentMessage.Event);
            Assert.AreEqual(0, deserializesSentMessage.MessageId);
            Assert.AreEqual(0, deserializesSentMessage.ReplyThreadId);
            Assert.IsTrue(communicator.GetIsBroadcast());
        }

        [Test]
        public void Receive_DownloadingAFile_FileShouldBeFetchedAndForwadedToTheCommunicator()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            var file = new SendFileData(pathA);

            var fileDownloadMessage = new MessageData
            {
                Message = "a.pdf",
                MessageId = 1,
                ReplyThreadId = 1,
                Type = MessageType.File,
                Event = MessageEvent.Download,
                SenderId = 10
            };

            var serializedFileDownloadMessage = serializer.Serialize(fileDownloadMessage);

            contentServer.Receive(serializedFileDownloadMessage);

            var sentData = communicator.GetSentData();

            var deserializedSentData = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual("a.pdf", deserializedSentData.Message);
            Assert.AreEqual(1, deserializedSentData.MessageId);
            Assert.AreEqual(1, deserializedSentData.ReplyThreadId);
            Assert.AreEqual(MessageType.File, deserializedSentData.Type);
            Assert.AreEqual(MessageEvent.Download, deserializedSentData.Event);
            Assert.AreEqual(file.fileName, deserializedSentData.FileData.fileName);
            Assert.AreEqual(file.fileSize, deserializedSentData.FileData.fileSize);
            Assert.AreEqual(file.fileContent, deserializedSentData.FileData.fileContent);

            var receivers = communicator.GetRcvIds();
            Assert.AreEqual(1, receivers.Count);
            Assert.AreEqual("10", receivers[0]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }

        [Test]
        public void
            Receive_HandlingPrivateMessages_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            var messageData = utils.GenerateNewMessageData("Hello");
            messageData.ReceiverIds = new[] {2, 3};
            messageData.SenderId = 1;

            var serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

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

            var receivers = communicator.GetRcvIds();
            Assert.AreEqual(3, receivers.Count);
            Assert.AreEqual("1", receivers[2]);
            Assert.AreEqual("2", receivers[0]);
            Assert.AreEqual("3", receivers[1]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }

        [Test]
        public void Receive_InvalidEventForChatType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            var previousMessageToCommunicator = communicator.GetSentData();

            var eventMessage = new MessageData
            {
                MessageId = 0,
                ReplyThreadId = 0,
                Event = MessageEvent.Download,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_InvalidEventForFileType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            var previousMessageToCommunicator = communicator.GetSentData();

            var eventMessage = new MessageData
            {
                MessageId = 1,
                ReplyThreadId = 1,
                Event = MessageEvent.Star,
                Type = MessageType.File
            };

            var serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);

            eventMessage = new MessageData
            {
                MessageId = 1,
                ReplyThreadId = 1,
                Event = MessageEvent.Update,
                Type = MessageType.File
            };

            serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void
            Receive_StarringAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            var previousMessageToCommunicator = communicator.GetSentData();

            var starMessage = new MessageData
            {
                MessageId = 10,
                ReplyThreadId = 10,
                Event = MessageEvent.Star,
                Type = MessageType.Chat
            };

            var serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void
            Receive_UpdatingAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            var previousMessageToCommunicator = communicator.GetSentData();

            var updateMessage = new MessageData
            {
                MessageId = 10,
                ReplyThreadId = 10,
                Event = MessageEvent.Update,
                Type = MessageType.Chat,
                Message = "Hello There!"
            };

            var serializedStarMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_GettingInvalidDataFromNotificationListener_ShouldReturnGracefully()
        {
            var previousMessageToCommunicator = communicator.GetSentData();

            var garbageData = " adfasfasfsadf";
            contentServer.Receive(garbageData);

            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void SGetAllMessages_GettingAllTheMessagesOnServer_ShouldReturnListOfChatContextsWithAllTheMessages()
        {
            var chatContexts = contentServer.SGetAllMessages();

            var firstMessage = chatContexts[0].MsgList[0];

            Assert.AreEqual("First Message", firstMessage.Message);
            Assert.AreEqual(MessageType.Chat, firstMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, firstMessage.Event);
            Assert.AreEqual(0, firstMessage.MessageId);
            Assert.AreEqual(0, firstMessage.ReplyThreadId);

            var secondMessage = chatContexts[1].MsgList[0];

            Assert.AreEqual("Test_File.pdf", secondMessage.Message);
            Assert.AreEqual(MessageType.File, secondMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, secondMessage.Event);
            Assert.AreEqual(1, secondMessage.MessageId);
            Assert.AreEqual(1, secondMessage.ReplyThreadId);
        }

        [Test]
        public void
            SSendAllMessagesToClient_SendingAllMessagesToANewlyJoinedClient_ListOfChatContextsShouldBeForwadedToCommunicator()
        {
            communicator.Reset();

            var messageData = new MessageData
            {
                Type = MessageType.HistoryRequest,
                SenderId = 10
            };

            var serializedMessageData = serializer.Serialize(messageData);

            contentServer.Receive(serializedMessageData);

            var serializedAllMessages = communicator.GetSentData();

            var chatContexts = serializer.Deserialize<List<ChatContext>>(serializedAllMessages);

            var firstMessage = chatContexts[0].MsgList[0];

            Assert.AreEqual("First Message", firstMessage.Message);
            Assert.AreEqual(MessageType.Chat, firstMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, firstMessage.Event);
            Assert.AreEqual(0, firstMessage.MessageId);
            Assert.AreEqual(0, firstMessage.ReplyThreadId);

            var secondMessage = chatContexts[1].MsgList[0];

            Assert.AreEqual("Test_File.pdf", secondMessage.Message);
            Assert.AreEqual(MessageType.File, secondMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, secondMessage.Event);
            Assert.AreEqual(1, secondMessage.MessageId);
            Assert.AreEqual(1, secondMessage.ReplyThreadId);

            var receiverIds = communicator.GetRcvIds();
            Assert.AreEqual(1, receiverIds.Count);
            Assert.AreEqual("10", receiverIds[0]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }
    }
}