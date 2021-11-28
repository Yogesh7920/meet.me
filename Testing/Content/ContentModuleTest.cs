/// <author>Sahil J. Chaudhari</author>
/// <created>20/11/2021</created>
/// <modified>24/11/2021</modified>
/// <summary>
/// This file contains all required methods and tests for module testing
/// </summary>

using Content;
using Networking;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace Testing.Content
{
    [TestFixture]
    public class ContentModuleTesting
    {
        
        [SetUp]
        public void Setup()
        {
            contentClient = ContentClientFactory.GetInstance() as ContentClient;
            iContentClient = contentClient;
            util = new Utils();
            serializer = new Serializer();
            fakeCommunicator = new FakeCommunicator();
            fakeListener = new FakeContentListener();
            iFakeListener = fakeListener;
            notificationHandler = new ContentClientNotificationHandler(contentClient);
            contentServer = ContentServerFactory.GetInstance() as ContentServer;
            iContentServer = contentServer;
            iFakeServerListener = fakeListener;
        }

        /// <summary>
        /// Checking singleton pattern of content client factory
        /// Both get instances should be same
        /// </summary>
        [Test]
        public void GetInstance_ContentClientFactory_IContentClientShouldBeSingleton()
        {
            IContentClient client1 = ContentClientFactory.GetInstance();
            IContentClient client2 = ContentClientFactory.GetInstance();
            Assert.AreEqual(client1, client2);
        }

        /// <summary>
        /// Calling SetUser function of content client factory to assign user id given by session manager to content client instance
        /// </summary>
        [Test]
        public void SetUser_ContentClientFactory_UserIdOfContentClientShouldMatchWithGivenID()
        {
            int userId = 1001;
            ContentClient contentClientInstance = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(userId);
            Assert.AreEqual(userId, contentClientInstance.UserId);
        }

        /// <summary>
        /// Checking singleton pattern of content server factory
        /// Both get instances should be same
        /// </summary>
        [Test]
        public void GetInstance_ContentServerFactory_IContentServerShouldBeSingleton()
        {
            IContentServer server1 = ContentServerFactory.GetInstance();
            IContentServer server2 = ContentServerFactory.GetInstance();
            Assert.AreEqual(server1, server2);
        }

        /// <summary>
        /// GetUserId returns user id of contentClient instance required for UX
        /// To test, first set user id and then get it using GetUserId method, both should match 
        /// </summary>
        [Test]
        public void GetUserId_GettingContentClientUserId_UserIdOfContentClientShouldMatchWithReturnedID()
        {
            int userId = 1001;
            ContentClient contentClientInstance = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(userId);
            IContentClient iContentClientInstance = ContentClientFactory.GetInstance();
            int rcvUserId = iContentClientInstance.GetUserId();
            Assert.AreEqual(rcvUserId, contentClientInstance.UserId);
        }

        /// <summary>
        /// This test sends invalid type supported by content client, should raise exception
        /// </summary>
        [Test]
        public void CSend_InvalidTypeSend_ShouldThrowException()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: (MessageType)2);
            contentClient.UserId = userId;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual("Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File", ex.Message);
        }

        /// <summary>
        /// In this test, we are giving reply thread Id that doesn't exist in CSend method
        /// </summary>
        [Test]
        public void CSend_ReplyThreadIdDoesNotExist_ShouldThrowException()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, replyId: 101);
            contentClient.UserId = userId;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.That(ex.Message.Contains("thread id"));
        }

        [Test]
        public void CSend_ReceiverIdNull_ShouldThrowException()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?");
            sampleData.ReceiverIds = null;
            contentClient.UserId = userId;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual(ex.Message.Contains("ids given is null"), true);
        }

        /// <summary>
        /// This test will send simple message using CSend method, we will check whether client is sending proper request to server
        /// by fetching msg from communicator, deserialize it and compare fields of Messagedata, same approach for following tests.
        /// We are sending private msg to user with id 1002
        /// </summary>
        [Test]
        public void CSend_ChatSendingHiMsg_SerializedStringShouldMatchInputMsg()
        {
            int UserId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            iContentClient.CSend(sampleData);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, sampleMsgData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData, sampleMsgData.FileData);
                Assert.AreEqual(receivedMessage.Starred, sampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// This test will check CSend method by sending msg with newline to check whether it support escape sequences
        /// Msg sent to communicator should have same msg with msg given to csend
        /// </summary>
        [Test]
        public void CSend_ChatSendingMsgWithNewline_SerializedStringShouldMatchInputMsg()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            iContentClient.CSend(sampleData);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, sampleMsgData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData, sampleMsgData.FileData);
                Assert.AreEqual(receivedMessage.Starred, sampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Sending null msg in CSend method, which is invalid, exception will be thrown
        /// </summary>
        [Test]
        public void CSend_ChatSendingMsgWithNullString_InvalidExceptionShouldThrown()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual("Invalid Message String", ex.Message);
        }

        /// <summary>
        /// Sending msg in CSend method, which is replying to thread that doesn't exist , exception will be thrown
        /// </summary>
        [Test]
        public void CSend_ChatSendingMsgWithNotExistReplyThreadId_ExceptionWillRaise()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            sampleData.ReplyThreadId = 1001;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual(ex.Message.Contains("Thread with given reply thread id"), true);
        }

        /// <summary>
        /// Sending msg in CSend method, which is replying to message id that doesn't exist, exception will be thrown
        /// </summary>
        [Test]
        public void CSend_ChatSendingMsgWithFalseReplyMsgId_ExceptionWillRaise()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            sampleData.ReplyMsgId = 1001;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual(ex.Message.Contains("Invalid reply message id"), true);
        }

        /// <summary>
        /// In this test we are trying to send reply to exist thread id but non exist message Id which will throw exception
        /// </summary>
        [Test]
        public void CSend_ReplyToExistThreadIdInvalidReplyMsgId_ExceptionWillRaise()
        {
            // Generating content client instance with fake communicator and also generating iContentClient using contentClient
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator newFakeCommunicator = new FakeCommunicator();
            contentClient.Communicator = newFakeCommunicator;
            int userId = 1001;
            contentClient.UserId = userId;
            IContentClient iContentClient = contentClient;

            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            MessageData dataToSerialize = util.GenerateNewMessageData("Hello", MessageId: 512, ReplyThreadId: 13);

            // Notifying client with msg
            newFakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(50);

            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();

            // Generating and sending msg to reply thread of received msg but incorrect replyMsgId
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { }, type: MessageType.Chat);
            sampleData.ReplyMsgId = 1203;
            sampleData.ReplyThreadId = listenedData.ReplyThreadId;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual(ex.Message.Contains("Invalid reply message id"), true);
        }

        /// <summary>
        /// In this test we are trying to send reply to exist message which is private and hence when we are sending reply request to server then receiver Id field of message data
        /// should have sender of message that we are replying to
        /// </summary>
        [Test]
        public void CSend_ReplyToExistPrivateMessage_ReceiverIdFieldsMustHaveMsgSenderId()
        {
            // Generating content client instance with fake communicator and also generating iContentClient using contentClient
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator newFakeCommunicator = new FakeCommunicator();
            contentClient.Communicator = newFakeCommunicator;
            int userId = 1001;
            contentClient.UserId = userId;
            IContentClient iContentClient = contentClient;

            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            // Generating private message where user 2001 sending hello to user 1001
            MessageData dataToSerialize = util.GenerateNewMessageData("Hello", MessageId: 490, ReplyThreadId: 6, rcvIds: new int[] {userId}, SenderId: 2001);

            // Notifying client with msg
            newFakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(50);

            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();

            // Generating and sending msg to reply thread of received msg but incorrect replyMsgId
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] {2001 }, type: MessageType.Chat);
            sampleData.ReplyMsgId = listenedData.MessageId;
            sampleData.ReplyThreadId = listenedData.ReplyThreadId;
            iContentClient.CSend(sampleData);
            var sendSerializedMsg = newFakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            Assert.AreEqual(deserialized.Message, sampleData.Message);
            Assert.AreEqual(deserialized.ReceiverIds, sampleData.ReceiverIds);
        }

        /// <summary>
        /// In this test we are trying to send reply to exist message which is private and hence when we are sending reply request to server then receiver Id field of message data
        /// should have all users that are msg was sent to
        /// </summary>
        [Test]
        public void CSend_ReplyToExistPrivateMessageWithMultipleReceivers_ReceiverIdFieldsMustHaveAllUsersRelatedToMsg()
        {
            // Generating content client instance with fake communicator and also generating iContentClient using contentClient
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator newFakeCommunicator = new FakeCommunicator();
            contentClient.Communicator = newFakeCommunicator;
            int userId = 1001;
            contentClient.UserId = userId;
            IContentClient iContentClient = contentClient;

            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            // Generating private message where user 2001 sending hello to user 1001, 1002 and 1003
            MessageData dataToSerialize = util.GenerateNewMessageData("Hello", MessageId: 491, ReplyThreadId: 5, rcvIds: new int[] { userId, 1002, 1003 }, SenderId: 2001);

            // Notifying client with msg
            newFakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(50);

            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();

            // Generating and sending msg to reply all users related to msgId
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 2001, 1002, 1003}, type: MessageType.Chat);
            sampleData.ReplyMsgId = listenedData.MessageId;
            sampleData.ReplyThreadId = listenedData.ReplyThreadId;
            iContentClient.CSend(sampleData);
            var sendSerializedMsg = newFakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            Assert.AreEqual(deserialized.Message, sampleData.Message);
            Assert.AreEqual(deserialized.ReceiverIds, sampleData.ReceiverIds);
        }

        /// <summary>
        /// Sending msg for broadcast, receiver ID list will be empty indicating broadcast, msg should match with one send over
        /// fake communicator
        /// </summary>
        [Test]
        public void CSend_ChatSendingHiMsgWithBroadcast_SerializedStringShouldMatchInputMsg()
        {
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);

            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;

            iContentClient.CSend(sampleData);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, sampleMsgData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData, sampleMsgData.FileData);
                Assert.AreEqual(receivedMessage.Starred, sampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// sending file using CSend, sent msg should have all info about file sent
        /// </summary>
        [Test]
        public void CSend_FileSendingValidFilePathToServer_SerializedStringShouldMatchFileData()
        {
            int userId = 1001;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var filedata = new SendFileData(filePath);
            SendMessageData sampleData = util.GenerateChatSendMsgData(filePath, new int[] { }, type: MessageType.File);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, filePath, new int[] { }, type: MessageType.File);

            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            iContentClient.CSend(sampleData);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, filedata.fileName);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData.fileContent, filedata.fileContent);
                Assert.AreEqual(receivedMessage.FileData.fileSize, filedata.fileSize);
                Assert.AreEqual(receivedMessage.FileData.fileName, filedata.fileName);
                Assert.AreEqual(receivedMessage.Starred, sampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, -1);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CSend_InValidFilePath_ShouldThrowFileNotFoundException()
        {
            int userId = 1001;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = ".\\InvalidFile.pdf";
            SendMessageData sampleData = util.GenerateChatSendMsgData(filePath, new int[] { }, type: MessageType.File);
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            FileNotFoundException ex1 = Assert.Throws<FileNotFoundException>(() => new SendFileData(filePath));
            Assert.AreEqual(ex1.Message.Contains(" not found"), true);
            FileNotFoundException ex2 = Assert.Throws<FileNotFoundException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual("File " + filePath + " not found", ex2.Message);
        }

        /// <summary>
        /// marking star msg already exist in memory, first sending msg using CSend and then caling CMarkstar over it
        /// msg received from fake communicator should have same msgID and star event
        /// </summary>
        [Test]
        public void CMarkStar_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperMarkstarReq()
        {
            int userId = 1001;
            int msgId = 13;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            sampleMsgData.MessageId = msgId;
            sampleMsgData.ReplyThreadId = 1;
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgData));
            System.Threading.Thread.Sleep(10);
            iContentClient.CMarkStar(msgId);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Star);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, msgId);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Following test are all invalid testcases for CMarkStar that will throw exceptions
        /// </summary>
        [Test]
        public void CMarkStar_StarringNotExistMsg_ShouldThrowException()
        {
            int userId = 1001;
            int msgId = 16;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CMarkStar(msgId));
            Assert.AreEqual("Message with given message id doesn't exist", ex.Message);
        }

        [Test]
        public void CMarkStar_StarringFileTypeMsg_ShouldThrowException()
        {
            int userId = 1001;
            int msgId = 15;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            MessageData sampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage, filePath, new int[] { }, type: MessageType.File, replyId: 1);
            sampleMsgDataSend.MessageId = msgId;
            sampleMsgDataSend.SenderId = userId;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CMarkStar(msgId));
            Assert.AreEqual("Message with given message id is not chat", ex.Message);
        }

        /// <summary>
        /// Following test are all invalid testcases for CUpdateChat that will throw exceptions
        /// </summary>
        [Test]
        public void CUpdate_StarringNotExistMsg_ShouldThrowException()
        {
            int userId = 1001;
            int msgId = 16;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CUpdateChat(msgId, "Hi"));
            Assert.AreEqual("Message with given message id doesn't exist", ex.Message);
        }

        [Test]
        public void CUpdate_UpdatingFileTypeMsg_ShouldThrowException()
        {
            int userId = 1001;
            int msgId = 17;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            MessageData sampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage, filePath, new int[] { }, type: MessageType.File, replyId: 1);
            sampleMsgDataSend.MessageId = msgId;
            sampleMsgDataSend.SenderId = userId;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CUpdateChat(msgId, "Hi"));
            Assert.AreEqual("Message type is not chat", ex.Message);
        }

        [Test]
        public void CUpdate_UpdatingOtherUsersMsg_ShouldThrowException()
        {
            int userId = 1001;
            int otherUserId = 1005;
            int msgId = 18;
            // Generating Message Data with sender Id as other user Id i.e 1005
            MessageData sampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage,"Hello", new int[] { }, type: MessageType.Chat, replyId: 1);
            sampleMsgDataSend.MessageId = msgId;
            sampleMsgDataSend.SenderId = otherUserId;
            // Setting user Id of client as 1001
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            // Notifying fakecommunicator abt other user's message to notify content client which will update its own memory tables
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            // updating othe user's msg using CUpdateChat method
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CUpdateChat(msgId, "Hi"));
            Assert.AreEqual("Update not allowed for messages from another sender", ex.Message);
        }

        /// <summary>
        /// updating msg already exist in memory, first sending msg using CSend and then caling CUpdate over it
        /// msg received from fake communicator should have same msgID, updated msg and update event
        /// </summary>
        [Test]
        public void CUpdate_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperUpdateReq()
        {
            int userId = 1001;
            int msgId = 12;
            string updateChat = "Hi, This is updated msg.";
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            sampleMsgData.MessageId = msgId;
            sampleMsgData.SenderId = userId;
            sampleMsgData.ReplyThreadId = 1;
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgData));
            System.Threading.Thread.Sleep(10);
            iContentClient.CUpdateChat(msgId, updateChat);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Update);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, msgId);
                Assert.AreEqual(receivedMessage.Message, updateChat);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// We are sending invalid msg id in CDownload which will throw exception
        /// </summary>
        [Test]
        public void CDownload_SendingDownloadReqToServerWithInvalidMsgId_ShouldThrowArgumentException()
        {
            int userId = 1001;
            int msgId = 100;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string savePath = currentDirectory + "\\SavedTestFile.pdf";
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CDownload(msgId, savePath));
            Assert.AreEqual("Message with given message ID not found", ex.Message);
        }

        /// <summary>
        /// Savepath doesnot exist
        /// </summary>
        [Test]
        public void CDownload_InvalidSavePath_ShouldThrowArgumentException()
        {
            int userId = 1001;
            int msgId = 100;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string savePath = currentDirectory + "\\doesNotExist\\SavedTestFile.pdf";
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CDownload(msgId, savePath));
            Assert.AreEqual("Given file path is not writable", ex.Message);
        }

        /// <summary>
        /// We will send msgId which will have chat type hence should throw exception
        /// </summary>
        [Test]
        public void CDownload_NonFileType_ShouldThrowArgumentException()
        {
            int msgId = 10;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string savePath = currentDirectory + "\\SavedTestFile.pdf";
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            MessageData dataToSerialize = util.GenerateNewMessageData("Hello", ReplyThreadId: 10, MessageId: msgId);
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(10);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CDownload(msgId, savePath));
            Assert.That(ex.Message.Contains("not a file"));
        }

        /// <summary>
        /// we will send download request to server with valid msgId of file type over fake communicator and will fetch msg from there
        /// and compare field to see if valid request sent.
        /// </summary>
        [Test]
        public void CDownload_ValidFileMsgExistInDataBase_ShouldSendProperReqToServer()
        {
            int userId = 1001;
            int msgId = 11;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            MessageData sampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage, filePath, new int[] { }, type: MessageType.File, replyId: 1);
            sampleMsgDataSend.MessageId = msgId;
            sampleMsgDataSend.SenderId = userId;
            string savePath = currentDirectory + "\\SavedTestFile.pdf";
            MessageData sampleMsgDataDownload = util.GenerateChatMessageData(MessageEvent.Download, savePath, new int[] { }, type: MessageType.File);
            sampleMsgDataDownload.MessageId = msgId;
            sampleMsgDataDownload.SenderId = userId;
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            fakeCommunicator.Notify(serializer.Serialize(sampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            iContentClient.CDownload(msgId, savePath);
            System.Threading.Thread.Sleep(10);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized;
                Assert.AreEqual(receivedMessage.Message, sampleMsgDataDownload.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Download);
                Assert.AreEqual(receivedMessage.Type, sampleMsgDataDownload.Type);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, msgId);
                Assert.AreEqual(receivedMessage.FileData, null);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// This test will check whether contentClient handles subscription, by checking whether subscriber receive msg
        /// on CSubscribe content client. We will send msg using fakecommunicator to notify Content client who will notify it to subscriber
        /// </summary>
        [Test]
        public void CSubscribe_SubcribingToContentClient_SubscriberShouldGetMsgOnNotify()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            string msg = "Hello";
            MessageData dataToSerialize = util.GenerateNewMessageData(msg, MessageId: 401, ReplyThreadId: 1);
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(10);
            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();
            Assert.AreEqual(listenedData.Message, msg);
        }

        ///// <summary>
        ///// This test will check whether contentClient handles multiple subscription, by checking whether all subscriber receive msg
        ///// </summary>
        [Test]
        public void CSubscribe_MultipleSubcribingToContentClient_SubscriberShouldGetMsgOnNotify()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            FakeContentListener fakeListener1 = new FakeContentListener();
            IContentListener iFakeListener1 = fakeListener1;
            FakeContentListener fakeListener2 = new FakeContentListener();
            IContentListener iFakeListener2 = fakeListener2;
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener1);
            iContentClient.CSubscribe(iFakeListener2);
            string msg = "Hello";
            MessageData dataToSerialize = util.GenerateNewMessageData(msg, MessageId: 402, ReplyThreadId: 1);
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(10);
            // Fetching listened data from listener
            ReceiveMessageData listenedData1 = fakeListener1.GetOnMessageData();
            ReceiveMessageData listenedData2 = fakeListener2.GetOnMessageData();
            Assert.AreEqual(listenedData1.Message, msg);
            Assert.AreEqual(listenedData2.Message, msg);
        }

        /// <summary>
        /// listening unsupported datatype by content, should throw exception
        /// </summary>
        [Test]
        public void OnDataReceived_UnSupportedObject_ShouldThrowException()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            List<int> dataToSerialize = new List<int>();
            ArgumentException ex = Assert.Throws<ArgumentException>(() => fakeCommunicator.Notify(serializer.Serialize(dataToSerialize)));
            Assert.AreEqual(ex.Message.Contains("Deserialized object of unknown type"), true);
        }

        /// <summary>
        /// giving null messageData object to client, should throw exception
        /// </summary>
        [Test]
        public void OnDataReceived_NullMessageDataObject_ShouldThrowException()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            MessageData dataToSerialize = null;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => fakeCommunicator.Notify(serializer.Serialize(dataToSerialize)));
            Console.WriteLine(ex.Message);
            Assert.AreEqual(ex.Message.Contains("Received null message"), true);
        }

        /// <summary>
        /// giving null list of chatcontext object to client, should throw exception
        /// </summary>
        [Test]
        public void OnDataReceived_NullChatContextListObject_ShouldThrowException()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            List<ChatContext> dataToSerialize = null;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => fakeCommunicator.Notify(serializer.Serialize(dataToSerialize)));
            Console.WriteLine(ex.Message);
            Assert.AreEqual(ex.Message.Contains("Null message in argument"), true);
        }

        /// <summary>
        /// This test will check INotification handler for content client and IListener which will be used by UX in case of single msg arrival from server
        /// We also replying to received message by listener using thread ID and msg ID of listened msg using replyThreadId and replyMsgId field
        /// </summary>
        [Test]
        public void OnDataReceived_NewMessageWithReply_SameMsgShouldReceivedToSubscriber()
        {
            // Generating content client instance with fake communicator and also generating iContentClient using contentClient
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator newFakeCommunicator = new FakeCommunicator();
            contentClient.Communicator = newFakeCommunicator;
            int userId = 1001;
            contentClient.UserId = userId;
            IContentClient iContentClient = contentClient;

            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            MessageData dataToSerialize = util.GenerateNewMessageData("Hello", MessageId: 489, ReplyThreadId: 1);

            // Notifying client with msg
            newFakeCommunicator.Notify(serializer.Serialize(dataToSerialize));
            System.Threading.Thread.Sleep(50);

            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();
            Assert.AreEqual(listenedData.Message, dataToSerialize.Message);

            // Generating and sending msg to reply received msg
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] {}, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?", new int[] {}, type: MessageType.Chat);
            sampleData.ReplyMsgId = listenedData.MessageId;
            sampleMsgData.ReplyMsgId = listenedData.MessageId;
            sampleData.ReplyThreadId = listenedData.ReplyThreadId;
            sampleMsgData.ReplyThreadId = listenedData.ReplyThreadId;
            contentClient.CSend(sampleData);
            System.Threading.Thread.Sleep(50);

            // Fetching data from communicator that sending request to server
            var sendSerializedMsg = newFakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, sampleMsgData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData, sampleMsgData.FileData);
                Assert.AreEqual(receivedMessage.Starred, sampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleMsgData.ReceiverIds.Length);
                Assert.AreEqual(receivedMessage.ReplyMsgId, sampleMsgData.ReplyMsgId);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// This test will check INotification handler for content client and IListener which will be used by UX in case of multiple msg arrival from server
        /// </summary>
        [Test]
        public void OnDataReceived_MultipleNewMessage_SameMsgShouldReceivedToSubscriber()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            MessageData dataToSerialize1 = util.GenerateNewMessageData("Hello", MessageId: 123, ReplyThreadId: 1);
            MessageData dataToSerialize2 = util.GenerateNewMessageData("Hi", MessageId: 42, ReplyThreadId: 2);
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize1));
            System.Threading.Thread.Sleep(10);
            // Msg1
            ReceiveMessageData listenedData1 = fakeListener.GetOnMessageData();
            Assert.AreEqual(listenedData1.Message, dataToSerialize1.Message);
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize2));
            System.Threading.Thread.Sleep(10);
            // Msg2
            ReceiveMessageData listenedData2 = fakeListener.GetOnMessageData();
            Assert.AreEqual(listenedData2.Message, dataToSerialize2.Message);
        }

        /// <summary>
        /// In this test, we giving client custom made server response on download request, and checks whether client download that file to mentioned local directory
        /// First we are sending request to server using CSend and then changing event of msg to download and message to savepath and giving it back to client as server response
        /// </summary>
        [Test]
        public void OnDataReceived_DownloadMessage_FileShouldBeSaved()
        {            
            int UserId = 1001;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            string savePath = path[0] + "\\Testing\\Content\\Save_";
            SendMessageData sampleData = util.GenerateChatSendMsgData(filePath, new int[] { }, type: MessageType.File);
            contentClient.Communicator = fakeCommunicator;
            contentClient.UserId = UserId;
            iContentClient.CSend(sampleData);
            string sendSerializedMsg = fakeCommunicator.GetSentData();
            MessageData deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            deserialized.Message = savePath;
            deserialized.Event = MessageEvent.Download;
            fakeCommunicator.Notify(serializer.Serialize(deserialized));
            System.Threading.Thread.Sleep(50);
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// sending list of chat context to onDataReceived of INotificationHandler, will compare built chat context list with 
        /// chat context list we got by subsribing contentClient using fakeListener
        /// </summary>
        [Test]
        public void OnDataReceived_ListChatContext_SameChatContextsShouldReceivedToSubscriber()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            ReceiveMessageData dataToSerialize1 = util.GenerateNewReceiveMessageData("Hello", MessageId: 1, ReplyThreadId: 1);
            ReceiveMessageData dataToSerialize2 = util.GenerateNewReceiveMessageData("Hi", MessageId: 2, ReplyThreadId: 2);
            ReceiveMessageData dataToSerialize3 = util.GenerateNewReceiveMessageData("How are you? I am fine!", MessageId: 3, ReplyThreadId: 1);
            ChatContext chatList1 = new ChatContext();
            chatList1.ThreadId = 1;
            chatList1.MsgList.Add(dataToSerialize1);
            chatList1.MsgList.Add(dataToSerialize3);
            ChatContext chatList2 = new ChatContext();
            chatList2.MsgList.Add(dataToSerialize2);
            chatList2.ThreadId = 2;
            List<ChatContext> listCContext = new List<ChatContext>();
            listCContext.Add(chatList1);
            listCContext.Add(chatList2);
            string SerializedStr = serializer.Serialize(listCContext);
            fakeCommunicator.Notify(SerializedStr);
            System.Threading.Thread.Sleep(10);
            List<ChatContext> listenedData = fakeListener.GetOnAllMessagesData();
            for(int i=0; i<listenedData.Count; i++)
            {
                CompareChatContext(listenedData[i], listCContext[i]);
            }
        }

        /// <summary>
        /// getting messages on particular chat context thread, we will first store msg on memory of content client using OnDataReceived of fakeCommunicator
        /// and also building chat context and will compare returned chat context using CGetThread with built one.
        /// </summary>
        [Test]
        public void CGetThread_ReturnsChatContextOfGivenThreadIDMultipleThreads_ShouldMatchWithConstructedChatContext()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            MessageData dataToSerialize1 = util.GenerateNewMessageData("Hello", MessageId: 1, ReplyThreadId: 11);
            MessageData dataToSerialize2 = util.GenerateNewMessageData("Hi", MessageId: 2, ReplyThreadId: 12);
            MessageData dataToSerialize3 = util.GenerateNewMessageData("How are you? I am fine!", MessageId: 3, ReplyThreadId: 11);
            ChatContext chatList1 = new ChatContext();
            chatList1.ThreadId = 11;
            chatList1.MsgList.Add(dataToSerialize1);
            chatList1.MsgList.Add(dataToSerialize3);
            ChatContext chatList2 = new ChatContext();
            chatList2.MsgList.Add(dataToSerialize2);
            chatList2.ThreadId = 12;
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize1));
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize2));
            fakeCommunicator.Notify(serializer.Serialize(dataToSerialize3));
            ChatContext chatsOnContext1 = iContentClient.CGetThread(11);
            CompareChatContext(chatList1, chatsOnContext1);
            ChatContext chatsOnContext2 = iContentClient.CGetThread(12);
            CompareChatContext(chatList2, chatsOnContext2);
        }

        /// <summary>
        /// invalid thread i.e thread id does not exist, should throw exception
        /// </summary>
        [Test]
        public void CGetThread_InvalidThreadIdGiven_ShouldThrowException()
        {
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CGetThread(101));
            Assert.AreEqual("Thread with requested thread ID does not exist", ex.Message);
        }

        [Test]
        ///<summary>
        /// Here we are testing SGetAllMessages and SSendAllMessages of server by sending it three new message
        /// starring first message, updating second message and keeping third same which will be replies in context of first message
        /// This test will check new arrival message, broadcast, private sending, starring msg for chats
        /// Approach:
        /// 1. Generating three msg data
        /// 2. Server receiving msg 1 and star request for msg 1 using Receive function of server and then we also checking response send by server to client using
        /// GetMsgFromCommunicator method
        /// 3. similarly receiving msg 2 and update request over it and checking it with same method
        /// 4. last we sending msg3 which is a reply to msg1 and checking as mentioned above
        /// 5. then we are building chatcontext list over it and comparing that with one received via SGetAllMessages and SSendAllMessagesToClient 
        /// </summary>
        public void SGetAllMessagesAndSendAllMessages_GettingAllMsgsFromServer_ShouldMatchSentMsgsToServer()
        {
            contentServer.Communicator = fakeCommunicator;
            MessageData receiveMsgData1 = util.GenerateNewMessageData("Hello, how are you?", SenderId: 1001, MessageId: -1, ReplyThreadId: -1);
            MessageData receiveMsgData2 = util.GenerateNewMessageData("I am fine, How aboid u?", SenderId: 1002, MessageId: -1, ReplyThreadId: -1);
            MessageData receiveMsgData3 = util.GenerateNewMessageData("I am fine", SenderId: 1003, MessageId: -1, ReplyThreadId: -1);
            contentServer.Receive(serializer.Serialize(receiveMsgData1));
            MessageData msg1 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            TestMsgDataFieldsServer(msg1, receiveMsgData1);
            MessageData starMsg1 = msg1;
            starMsg1.Event = MessageEvent.Star;
            contentServer.Receive(serializer.Serialize(starMsg1));
            MessageData starReplyMsg1 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            TestMsgDataFieldsServer(msg1, starReplyMsg1);
            contentServer.Receive(serializer.Serialize(receiveMsgData2));
            MessageData msg2 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            TestMsgDataFieldsServer(msg2, receiveMsgData2);
            MessageData updateMsg2 = msg2;
            updateMsg2.Event = MessageEvent.Update;
            updateMsg2.Message = "I am fine, How about u?";
            contentServer.Receive(serializer.Serialize(updateMsg2));
            MessageData updateReplyMsg2 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            TestMsgDataFieldsServer(updateMsg2, msg2);
            Assert.AreEqual(true, starReplyMsg1.Starred);
            Assert.AreEqual(updateReplyMsg2.Message, "I am fine, How about u?");
            receiveMsgData3.ReplyThreadId = msg1.ReplyThreadId;
            contentServer.Receive(serializer.Serialize(receiveMsgData3));
            MessageData msg3 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            TestMsgDataFieldsServer(msg3, receiveMsgData3);

            // reflect updates in sent messages as they should be in the database if the database functions correctly
            // the messages will get newly generated ids in the database, so reflect those changes
            receiveMsgData1.MessageId = msg1.MessageId;
            receiveMsgData1.ReplyThreadId = msg1.ReplyThreadId;
            receiveMsgData1.ReplyMsgId = msg1.ReplyMsgId;
            // message 1 was later starred, so reflect that change
            receiveMsgData1.Starred = !receiveMsgData1.Starred;

            //similarly for message 2
            receiveMsgData2.MessageId = msg2.MessageId;
            receiveMsgData2.ReplyThreadId = msg2.ReplyThreadId;
            receiveMsgData2.ReplyMsgId = msg2.ReplyMsgId;
            // update message 2
            receiveMsgData2.Message = "I am fine, How about u?";

            ChatContext c1 = new ChatContext();
            c1.ThreadId = msg1.ReplyThreadId;
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(receiveMsgData1));
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(msg3));
            ChatContext c2 = new ChatContext();
            c2.ThreadId = msg2.ReplyThreadId;
            c2.MsgList.Add(util.MessageDataToReceiveMessageData(receiveMsgData2));
            chatList.Add(c1);
            chatList.Add(c2);
            iContentServer.SSendAllMessagesToClient(1003);
            TestSSendAllMessagesToClient(fakeCommunicator, serializer, chatList, 1003);
            CompareChatContextList(chatList, iContentServer.SGetAllMessages());
        }

        [Test]
        ///<summary>
        /// Here we are testing file related functionality of server, i.e storing new file message and handling donwload request
        /// Approach:
        /// 1. first we are sending file via CSend method of clinet
        /// 2. Receiving same serialized msg sent by client on server using Receive method
        /// 3. Fetching response sent by server using GetMsgFromCommunicator method and Sending it to client using OnReceive method of INotificationHandler
        /// 4. Client makes download request of same file using CDownload method and server receives it using Receive method
        /// 5. Server sends response which is fetched using GetMsgFromCommunicator method and received by client using OnReceive method
        /// 6. File is getting downloaded to local system which we are testing using File.Exists method
        /// </summary>
        public void SendingAndReceivingFileServer_NewFileMessageAndDownloadRequest_FileShouldBeDownloadedOnClient()
        {
            contentServer.Communicator = fakeCommunicator;
            int UserId = 1001;
            string currentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = currentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string filePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            string savePath = path[0] + "\\Testing\\Content\\Save_";
            var Filedata = new SendFileData(filePath);
            SendMessageData sampleData = util.GenerateChatSendMsgData(filePath, new int[] { }, type: MessageType.File);
            contentClient.Communicator = fakeCommunicator;
            contentClient.UserId = UserId;
            iContentClient.CSend(sampleData);
            string sendSerializedMsg = fakeCommunicator.GetSentData();
            MessageData sendNewFileData = serializer.Deserialize<MessageData>(sendSerializedMsg);
            contentServer.Receive(sendSerializedMsg);
            MessageData fileReplyMsg = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            ChatContext c1 = new ChatContext();
            c1.ThreadId = fileReplyMsg.ReplyThreadId;
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(fileReplyMsg));
            chatList.Add(c1);
            contentClient.OnReceive(fileReplyMsg);
            iContentClient.CDownload(fileReplyMsg.MessageId, savePath);
            string downloadReqMsg = fakeCommunicator.GetSentData();
            contentServer.Receive(downloadReqMsg);
            List<int> rcvId = new List<int>();
            rcvId.Add(UserId);
            MessageData fileReturnedData = GetMsgFromCommunicator(fakeCommunicator, serializer, false, rcvId);
            TestFile(fileReturnedData, savePath, sendNewFileData, fileReplyMsg.MessageId);
            contentClient.OnReceive(fileReturnedData);
            System.Threading.Thread.Sleep(50);
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }


        /// <summary>
        /// helper function to check file data of message data field
        /// </summary>
        public void TestFile(MessageData m1, string savePath, MessageData fileData, int msgId)
        {
            Assert.AreEqual(m1.FileData.fileContent, fileData.FileData.fileContent);
            Assert.AreEqual(m1.Message, savePath);
            Assert.AreEqual(m1.FileData.fileName, fileData.FileData.fileName);
            Assert.AreEqual(m1.MessageId, msgId);
        }

        /// <summary>
        /// subscribing to server using SSubscribe method given by IContentServer, subscriber should get proper msg on calling 
        ///  notify, which will be triggered when server will call Receive
        /// </summary>
        [Test]
        public void SSubscribe_SubcribingToContentServer_SubscriberShouldGetMsgOnNotify()
        {
            // Subscribing to content client
            iContentServer.SSubscribe(iFakeServerListener);
            // Building receiveMessageData to notify to subscribers
            MessageData receivedData = new MessageData();
            string msg = "hello";  // data will have m hello
            receivedData.Message = msg;
            receivedData.ReplyThreadId = -1;
            receivedData.Type = MessageType.Chat;
            receivedData.Event = MessageEvent.NewMessage;
            receivedData.ReceiverIds = new int[0];
            // Notifying to subscribers
            contentServer.Receive(serializer.Serialize(receivedData));
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData listenedData = fakeListener.GetOnMessageData();
            Assert.AreEqual(listenedData.Message, msg);
        }

        /// <summary>
        /// This function compares message, senderID and type fields of two given message datas
        /// </summary>
        public void TestMsgDataFieldsServer(MessageData m1, MessageData m2)
        {
            Assert.AreEqual(m1.Message, m2.Message);
            Assert.AreEqual(m1.SenderId, m2.SenderId);
            Assert.AreEqual(m1.Type, m2.Type);
        }

        /// <summary>
        /// This function checks SSendAllMessage of server where it will deserialize string from communicator
        /// will check if msg sent to valid user in private manner and will compare List of chatcontexts
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="serializer"></param>
        /// <param name="chats"></param>
        /// <param name="userId"></param>
        public void TestSSendAllMessagesToClient(FakeCommunicator communicator, ISerializer serializer, List<ChatContext> chats, int userId)
        {
            string msg = communicator.GetSentData();
            List<string> rcvIds = communicator.GetRcvIds();
            bool broadcastFlag = communicator.GetIsBroadcast();
            // Checking for private send
            Assert.AreEqual(1, rcvIds.Count);
            Assert.AreEqual(true, rcvIds.Contains(userId.ToString()));
            Assert.AreEqual(broadcastFlag, false);
            List<ChatContext> contexts = serializer.Deserialize<List<ChatContext>>(msg);
            CompareChatContextList(contexts, chats);
        }

        /// <summary>
        /// This function fetched msg string sent over fake communicator and deserialize it into message object
        /// It also checks whether data sent was braodcasted and who are the receivers
        /// </summary>
        public MessageData GetMsgFromCommunicator(FakeCommunicator communicator, ISerializer serializer, bool isBroadcast, List<int> rcvIds)
        {
            if (rcvIds == null)
            {
                rcvIds = new List<int>();
            }
            string receivedMsg = communicator.GetSentData();
            MessageData messageData = serializer.Deserialize<MessageData>(receivedMsg);
            List<string> receiverIds = communicator.GetRcvIds();
            bool broadcastFlag = communicator.GetIsBroadcast();
            if (isBroadcast)
            {
                Assert.AreEqual(broadcastFlag, true);
                Assert.AreEqual(receiverIds.Count, 0);
            }
            else
            {
                Assert.AreEqual(broadcastFlag, false);
                Assert.AreEqual(receiverIds.Count, rcvIds.Count);
                foreach (int i in rcvIds)
                {
                    if (!receiverIds.Contains(i.ToString()))
                    {
                        Assert.Fail();
                    }
                }
            }

            return messageData;
        }

        /// <summary>
        /// This function compares receiveMsgData
        /// </summary>
        public void CompareReceiveMessageData(ReceiveMessageData m1, ReceiveMessageData m2)
        {
            Assert.AreEqual(m1.Message, m2.Message);
            Assert.AreEqual(m1.MessageId, m2.MessageId);
            Assert.AreEqual(m1.ReplyThreadId, m2.ReplyThreadId);
            Assert.AreEqual(m1.SenderId, m2.SenderId);
            Assert.AreEqual(m1.Starred, m2.Starred);
            Assert.AreEqual(m1.Type, m2.Type);
            Assert.AreEqual(m1.Event, m2.Event);
        }

        /// <summary>
        /// This function compares chat contexts
        /// </summary>
        public void CompareChatContext(ChatContext c1, ChatContext c2)
        {
            Assert.AreEqual(c1.ThreadId, c2.ThreadId);
            Assert.AreEqual(c1.MsgList.Count, c2.MsgList.Count);
            for (int i = 0; i < c1.MsgList.Count; i++)
            {
                CompareReceiveMessageData(c1.MsgList[i], c2.MsgList[i]);
            }
        }

        /// <summary>
        /// This function compared list of chat contexts
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        public void CompareChatContextList(List<ChatContext> l1, List<ChatContext> l2)
        {
            for (int i = 0; i < l1.Count; i++)
            {
                CompareChatContext(l1[i], l2[i]);
            }
        }

        /// <summary>
        /// This will keep track of all contexts created over server for testing purpose when we will call
        /// SGetAllMessages and SSendAllMessagesToClient
        /// </summary>
        List<ChatContext> chatList = new List<ChatContext>();
        /// <summary>
        /// Interfaces and objects for testing
        /// </summary>
        ContentClient contentClient; 
        IContentClient iContentClient;
        Utils util;
        ISerializer serializer;
        FakeCommunicator fakeCommunicator;
        IContentListener iFakeListener;
        FakeContentListener fakeListener;
        INotificationHandler notificationHandler;
        ContentServer contentServer;
        IContentServer iContentServer;
        IContentListener iFakeServerListener;

    }
}
