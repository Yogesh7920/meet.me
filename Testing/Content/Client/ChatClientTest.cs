/// <author>Vishesh Munjal</author>
/// <created>20/11/2021</created>
/// <summary>
/// This file contains Test for chatClient
/// </summary>

using System;
using Content;
using Networking;
using NUnit.Framework;

namespace Testing.Content
{
    public class ChatClientTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        ///     This Test checks the conversion function SendToMessage of the ChatClient
        /// </summary>
        [Test]
        public void Test_Converting_SendMessageObject_To_MessageData()
        {
            var util = new Utils();
            var sampleData = util.GenerateChatSendMsgData("Apple", new[] {1002}, type: MessageType.Chat);
            var conch = new ChatClient(util.GetFakeCommunicator());
            var x = conch.SendToMessage(sampleData, MessageEvent.NewMessage);

            Assert.AreEqual(x.Message, sampleData.Message);
            Assert.AreEqual(x.Event, MessageEvent.NewMessage);
            Assert.AreEqual(x.Type, sampleData.Type);
            Assert.AreEqual(x.FileData, null);
            Assert.AreEqual(x.Starred, false);
            Assert.AreEqual(x.ReplyThreadId, sampleData.ReplyThreadId);
            Assert.AreEqual(x.ReceiverIds.Length, sampleData.ReceiverIds.Length);
        }

        /// <summary>
        ///     This Test checks the ChatNewMessage  of the ChatClient by using a Message "Apple"
        /// </summary>
        [Test]
        public void Test_ChatNewMessage()
        {
            var util = new Utils();
            var userId = 1001;
            var sampleData = util.GenerateChatSendMsgData("Apple", new[] {1002}, type: MessageType.Chat);
            var sampleMsgData =
                util.GenerateChatMessageData(MessageEvent.NewMessage, "Apple", new[] {1002}, type: MessageType.Chat);

            ISerializer serializer = new Serializer();

            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatNewMessage(sampleData);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized;
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
        ///     This Test checks the ChatNewMessage function of the ChatClient in case of an Empty String.
        /// </summary>
        [Test]
        public void Test_ChatNewMessage_EmptyString()
        {
            var util = new Utils();
            var userId = 1001;
            var sampleData = util.GenerateChatSendMsgData("", new[] {1002}, type: MessageType.Chat);

            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;


            var ex = Assert.Throws<ArgumentException>(() => contentChat.ChatNewMessage(sampleData));
            var contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        ///     This Test checks the ChatNewMessage function of the ChatClient in case of an Null String.
        /// </summary>
        [Test]
        public void Test_ChatNewMessage_NullString()
        {
            var util = new Utils();
            var userId = 1001;
            var sampleData = util.GenerateChatSendMsgData(null, new[] {1002}, type: MessageType.Chat);


            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;


            var ex = Assert.Throws<ArgumentException>(() => contentChat.ChatNewMessage(sampleData));
            var contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        ///     This Test checks the ChatUpdate function of the ChatClient using a chat and updating it.
        /// </summary>
        [Test]
        public void Test_ChatUpdate()
        {
            var util = new Utils();
            var userId = 1001;
            var msgId = 10;
            var threadId = 10;
            var sampleMsgData =
                util.GenerateChatMessageData(MessageEvent.Update, "Banana", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();

            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatUpdate(msgId, threadId, "APPLE");

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized;
                Assert.AreEqual(receivedMessage.Message, "APPLE");
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Update);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, msgId);
                Assert.AreEqual(receivedMessage.ReplyThreadId, threadId);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        ///     This Test checks the ChatNewMessage function of the ChatClient in case of an Empty String.
        /// </summary>
        [Test]
        public void Test_ChatUpdate_EmptyString()
        {
            var util = new Utils();
            var userId = 1001;
            var sampleData = util.GenerateChatSendMsgData("", new[] {1002}, type: MessageType.Chat);

            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;


            var ex = Assert.Throws<ArgumentException>(() => contentChat.ChatUpdate(11, 1, ""));
            var contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        ///     This Test checks the ChatNewMessage function of the ChatClient in case of an Null String.
        /// </summary>
        [Test]
        public void Test_ChatUpdate_NullString()
        {
            var util = new Utils();
            var userId = 1001;
            var sampleData = util.GenerateChatSendMsgData(null, new[] {1002}, type: MessageType.Chat);


            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;


            var ex = Assert.Throws<ArgumentException>(() => contentChat.ChatUpdate(11, 1, null));
            var contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        ///     This Test checks the ChatStar function of the ChatClient.
        /// </summary>
        [Test]
        public void Test_Star()
        {
            var util = new Utils();
            var userId = 1001;
            var msgId = 10;
            var threadId = 11;
            var sampleMsgData =
                util.GenerateChatMessageData(MessageEvent.Update, "", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();

            var contentChat = new ChatClient(util.GetFakeCommunicator());
            var fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatStar(msgId, threadId);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Star);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, userId);
                Assert.AreEqual(receivedMessage.MessageId, msgId);
                Assert.AreEqual(receivedMessage.ReplyThreadId, threadId);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}