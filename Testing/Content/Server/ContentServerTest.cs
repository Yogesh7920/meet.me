using NUnit.Framework;
using Content;
using Networking;
using System.IO;
using System;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ContentServerTests
    {
        private ContentServer contentServer;
        private Utils utils;
        private FakeContentListener listener;
        private FakeCommunicator communicator;
        private ISerializer serializer;

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

            MessageData messageData = utils.GenerateNewMessageData("First Message");
            string serializedMessage = serializer.Serialize(messageData);
            contentServer.Receive(serializedMessage);

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

            serializedMessage = serializer.Serialize(file);

            contentServer.Receive(serializedMessage);
        }

        [Test]
        public void SSubscribe_SubsribingToNotification_ShouldBeAbleToGetNotificationOfNewMessages()
        {
            MessageData messageData = utils.GenerateNewMessageData("Hello");

            string serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

            ReceiveMessageData notifiedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello", notifiedMessage.Message);
            Assert.AreEqual(messageData.Type, notifiedMessage.Type);
            Assert.AreEqual(messageData.Event, notifiedMessage.Event);
            Assert.AreEqual(messageData.SenderId, notifiedMessage.SenderId);
            Assert.AreEqual(messageData.Starred, notifiedMessage.Starred);
            Assert.AreEqual(messageData.ReceiverIds, notifiedMessage.ReceiverIds);
        }

        [Test]
        public void Receive_HandlingNewMessage_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            MessageData messageData = utils.GenerateNewMessageData("Hello");

            string serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

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
        public void Receive_HandlingNewFile_ShouldSaveTheNewFileAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
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

            string serializesMessage = serializer.Serialize(file);

            contentServer.Receive(serializesMessage);

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
        public void Receive_StarringAMessage_ShouldStarTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            MessageData starMessage = new MessageData();
            starMessage.MessageId = 0;
            starMessage.ReplyThreadId = 0;
            starMessage.Event = MessageEvent.Star;
            starMessage.Type = MessageType.Chat;

            string serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            ReceiveMessageData starredMessage = listener.GetOnMessageData();

            Assert.AreEqual("First Message", starredMessage.Message);
            Assert.AreEqual(MessageType.Chat, starredMessage.Type);
            Assert.AreEqual(MessageEvent.Star, starredMessage.Event);
            Assert.AreEqual(0, starredMessage.MessageId);
            Assert.AreEqual(0, starredMessage.ReplyThreadId);
            Assert.IsTrue(starredMessage.Starred);

            string sentMessage = communicator.GetSentData();

            MessageData deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

            Assert.AreEqual("First Message", deserializesSentMessage.Message);
            Assert.AreEqual(MessageType.Chat, deserializesSentMessage.Type);
            Assert.AreEqual(MessageEvent.Star, deserializesSentMessage.Event);
            Assert.AreEqual(0, deserializesSentMessage.MessageId);
            Assert.AreEqual(0, deserializesSentMessage.ReplyThreadId);
            Assert.IsTrue(deserializesSentMessage.Starred);
            Assert.IsTrue(communicator.GetIsBroadcast());
        }

        [Test]
        public void Receive_UpdatingAMessage_ShouldUpdateTheMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            MessageData updateMessage = new MessageData();
            updateMessage.MessageId = 0;
            updateMessage.ReplyThreadId = 0;
            updateMessage.Event = MessageEvent.Update;
            updateMessage.Type = MessageType.Chat;
            updateMessage.Message = "Hello World!";

            string serializedUpdateMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedUpdateMessage);

            ReceiveMessageData updatedMessage = listener.GetOnMessageData();

            Assert.AreEqual("Hello World!", updatedMessage.Message);
            Assert.AreEqual(MessageType.Chat, updatedMessage.Type);
            Assert.AreEqual(MessageEvent.Update, updatedMessage.Event);
            Assert.AreEqual(0, updatedMessage.MessageId);
            Assert.AreEqual(0, updatedMessage.ReplyThreadId);

            string sentMessage = communicator.GetSentData();

            MessageData deserializesSentMessage = serializer.Deserialize<MessageData>(sentMessage);

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
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();

            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string pathA = path[0] + "\\Testing\\Content\\Test_File.pdf";

            SendFileData file = new SendFileData(pathA);

            MessageData fileDownloadMessage = new MessageData();
            fileDownloadMessage.Message = "a.pdf";
            fileDownloadMessage.MessageId = 1;
            fileDownloadMessage.ReplyThreadId = 1;
            fileDownloadMessage.Type = MessageType.File;
            fileDownloadMessage.Event = MessageEvent.Download;
            fileDownloadMessage.SenderId = 10;

            string serializedFileDownloadMessage = serializer.Serialize(fileDownloadMessage);

            contentServer.Receive(serializedFileDownloadMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);

            string sentData = communicator.GetSentData();

            MessageData deserializedSentData = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual("a.pdf", deserializedSentData.Message);
            Assert.AreEqual(1, deserializedSentData.MessageId);
            Assert.AreEqual(1, deserializedSentData.ReplyThreadId);
            Assert.AreEqual(MessageType.File, deserializedSentData.Type);
            Assert.AreEqual(MessageEvent.Download, deserializedSentData.Event);
            Assert.AreEqual(file.fileName, deserializedSentData.FileData.fileName);
            Assert.AreEqual(file.fileSize, deserializedSentData.FileData.fileSize);
            Assert.AreEqual(file.fileContent, deserializedSentData.FileData.fileContent);

            List<string> receivers = communicator.GetRcvIds();
            Assert.AreEqual(1, receivers.Count);
            Assert.AreEqual("10", receivers[0]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }

        [Test]
        public void Receive_HandlingPrivateMessages_ShouldSaveTheNewMessageAndNotifyTheSubcsribersAndForwardTheSerializedMessageToCommunicator()
        {
            MessageData messageData = utils.GenerateNewMessageData("Hello");
            messageData.ReceiverIds = new int[] { 2, 3 };
            messageData.SenderId = 1;

            string serializesMessage = serializer.Serialize(messageData);

            contentServer.Receive(serializesMessage);

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

            List<string> receivers = communicator.GetRcvIds();
            Assert.AreEqual(3, receivers.Count);
            Assert.AreEqual("1", receivers[2]);
            Assert.AreEqual("2", receivers[0]);
            Assert.AreEqual("3", receivers[1]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }

        [Test]
        public void Receive_InvalidEventForChatType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            MessageData eventMessage = new MessageData();
            eventMessage.MessageId = 0;
            eventMessage.ReplyThreadId = 0;
            eventMessage.Event = MessageEvent.Download;
            eventMessage.Type = MessageType.Chat;

            string serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_InvalidEventForFileType_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            MessageData eventMessage = new MessageData();
            eventMessage.MessageId = 1;
            eventMessage.ReplyThreadId = 1;
            eventMessage.Event = MessageEvent.Star;
            eventMessage.Type = MessageType.File;

            string serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);

            eventMessage = new MessageData();
            eventMessage.MessageId = 1;
            eventMessage.ReplyThreadId = 1;
            eventMessage.Event = MessageEvent.Update;
            eventMessage.Type = MessageType.File;

            serializedStarMessage = serializer.Serialize(eventMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_StarringAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            MessageData starMessage = new MessageData();
            starMessage.MessageId = 10;
            starMessage.ReplyThreadId = 10;
            starMessage.Event = MessageEvent.Star;
            starMessage.Type = MessageType.Chat;

            string serializedStarMessage = serializer.Serialize(starMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_UpdatingAMessageThatDoesNotExist_SubscribersShouldNotBeNotifiedAndNothingShouldBeSentToCommunicator()
        {
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            MessageData updateMessage = new MessageData();
            updateMessage.MessageId = 10;
            updateMessage.ReplyThreadId = 10;
            updateMessage.Event = MessageEvent.Update;
            updateMessage.Type = MessageType.Chat;
            updateMessage.Message = "Hello There!";

            string serializedStarMessage = serializer.Serialize(updateMessage);

            contentServer.Receive(serializedStarMessage);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void Receive_GettingInvalidDataFromNotificationListener_ShouldReturnGracefully()
        {
            ReceiveMessageData previousMessageToSubsribers = listener.GetOnMessageData();
            string previousMessageToCommunicator = communicator.GetSentData();

            string garbageData = " adfasfasfsadf";
            contentServer.Receive(garbageData);

            Assert.AreEqual(listener.GetOnMessageData(), previousMessageToSubsribers);
            Assert.AreEqual(communicator.GetSentData(), previousMessageToCommunicator);
        }

        [Test]
        public void SGetAllMessages_GettingAllTheMessagesOnServer_ShouldReturnListOfChatContextsWithAllTheMessages()
        {
            List<ChatContext> chatContexts = contentServer.SGetAllMessages();

            ReceiveMessageData firstMessage = chatContexts[0].MsgList[0];

            Assert.AreEqual("First Message", firstMessage.Message);
            Assert.AreEqual(MessageType.Chat, firstMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, firstMessage.Event);
            Assert.AreEqual(0, firstMessage.MessageId);
            Assert.AreEqual(0, firstMessage.ReplyThreadId);

            ReceiveMessageData secondMessage = chatContexts[1].MsgList[0];

            Assert.AreEqual("Test_File.pdf", secondMessage.Message);
            Assert.AreEqual(MessageType.File, secondMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, secondMessage.Event);
            Assert.AreEqual(1, secondMessage.MessageId);
            Assert.AreEqual(1, secondMessage.ReplyThreadId);
        }

        [Test]
        public void SSendAllMessagesToClient_SendingAllMessagesToANewlyJoinedClient_ListOfChatContextsShouldBeForwadedToCommunicator()
        {
            communicator.Reset();

            contentServer.SSendAllMessagesToClient(10);

            string serializedAllMessages = communicator.GetSentData();

            List<ChatContext> chatContexts = serializer.Deserialize<List<ChatContext>>(serializedAllMessages);

            ReceiveMessageData firstMessage = chatContexts[0].MsgList[0];

            Assert.AreEqual("First Message", firstMessage.Message);
            Assert.AreEqual(MessageType.Chat, firstMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, firstMessage.Event);
            Assert.AreEqual(0, firstMessage.MessageId);
            Assert.AreEqual(0, firstMessage.ReplyThreadId);

            ReceiveMessageData secondMessage = chatContexts[1].MsgList[0];

            Assert.AreEqual("Test_File.pdf", secondMessage.Message);
            Assert.AreEqual(MessageType.File, secondMessage.Type);
            Assert.AreEqual(MessageEvent.NewMessage, secondMessage.Event);
            Assert.AreEqual(1, secondMessage.MessageId);
            Assert.AreEqual(1, secondMessage.ReplyThreadId);

            List<string> receiverIds = communicator.GetRcvIds();
            Assert.AreEqual(1, receiverIds.Count);
            Assert.AreEqual("10", receiverIds[0]);
            Assert.IsFalse(communicator.GetIsBroadcast());
        }
    }
}