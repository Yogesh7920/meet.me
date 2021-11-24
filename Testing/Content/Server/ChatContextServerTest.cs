using Content;
using NUnit.Framework;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ChatContextServerTests
    {
        private ChatContextServer chatContextServer;
        private ContentDatabase database;
        private Utils _utils;

        [SetUp]
        public void Setup()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);
            _utils = new Utils();
        }

        [Test]
        public void Receive_HandlingNewMessage_StoreTheNewMessageAndReturnTheStoredMessage()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);
        }

        [Test]
        public void Receive_StarringAStoredMessage_MessageIsStarredAndReturnsTheStarredMessage()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData starMessage = new MessageData
            {
                MessageId = recv.MessageId,
                ReplyThreadId = recv.ReplyThreadId,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = chatContextServer.Receive(starMessage);

            Assert.IsNotNull(recv);
            Assert.AreEqual("Hello", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Star, recv.Event);
            Assert.IsTrue(recv.Starred);
        }

        [Test]
        public void Receive_StarringAMessageThatDoesNotExist_NullIsReturned()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData starMessage = new MessageData
            {
                MessageId = 1,
                ReplyThreadId = recv.ReplyThreadId,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = chatContextServer.Receive(starMessage);

            Assert.IsNull(recv);

            starMessage.MessageId = 0;
            starMessage.ReplyThreadId = 1;

            recv = chatContextServer.Receive(starMessage);

            Assert.IsNull(recv);
        }

        [Test]
        public void Receive_UpdatingAStoredMessage_MessageIsUpdatedAndReturnsTheUpdatedMessage()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData updateMessage = new MessageData
            {
                Message = "Hello World!",
                MessageId = recv.MessageId,
                ReplyThreadId = recv.ReplyThreadId,
                Type = MessageType.Chat,
                Event = MessageEvent.Update
            };

            recv = chatContextServer.Receive(updateMessage);

            Assert.IsNotNull(recv);
            Assert.AreEqual("Hello World!", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Update, recv.Event);
            Assert.IsFalse(recv.Starred);
        }

        [Test]
        public void Receive_UpdatingAMessageThatDoesNotExist_NullIsReturned()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData updateMessage = new MessageData
            {
                Message = "Hello World!",
                MessageId = 1,
                ReplyThreadId = recv.ReplyThreadId,
                Type = MessageType.Chat,
                Event = MessageEvent.Update
            };

            recv = chatContextServer.Receive(updateMessage);

            Assert.IsNull(recv);

            updateMessage.MessageId = 0;
            updateMessage.ReplyThreadId = 1;

            recv = chatContextServer.Receive(updateMessage);

            Assert.IsNull(recv);
        }

        [Test]
        public void Receive_ProvidingInvalidEventForChatType_NullIsReturned()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            message1.Event = MessageEvent.Download;

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.IsNull(recv);
        }

        [Test]
        public void Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned()
        {
            MessageData message1 = _utils.GenerateNewMessageData("Hello", SenderId: 1);

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData message2 = _utils.GenerateNewMessageData("Hello2", SenderId: 1, ReplyThreadId: message1.ReplyThreadId);

            recv = chatContextServer.Receive(message2);

            Assert.AreEqual(message2.Message, recv.Message);
            Assert.AreNotEqual(message2.MessageId, message1.MessageId);
            Assert.AreEqual(message2.Type, recv.Type);
            Assert.AreEqual(message2.SenderId, recv.SenderId);
            Assert.AreEqual(message2.Event, recv.Event);
            Assert.AreEqual(message2.ReplyThreadId, recv.ReplyThreadId);
            Assert.IsFalse(recv.Starred);

            MessageData message3 = _utils.GenerateNewMessageData("Hello3", SenderId: 1);

            recv = chatContextServer.Receive(message3);

            Assert.AreEqual(message3.Message, recv.Message);
            Assert.AreNotEqual(message3.MessageId, message2.MessageId);
            Assert.AreNotEqual(message3.MessageId, message1.MessageId);
            Assert.AreEqual(message3.Type, recv.Type);
            Assert.AreEqual(message3.SenderId, recv.SenderId);
            Assert.AreEqual(message3.Event, recv.Event);
            Assert.AreNotEqual(message2.ReplyThreadId, recv.ReplyThreadId);
            Assert.AreNotEqual(message1.ReplyThreadId, recv.ReplyThreadId);
            Assert.IsFalse(recv.Starred);
        }

        [Test]
        public void Receive_StarringMultipleMessages_OnlyTheGivenMessagesAreStarred()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);

            Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned();

            MessageData message1 = new MessageData
            {
                MessageId = 0,
                ReplyThreadId = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Star, recv.Event);
            Assert.IsTrue(recv.Starred);

            message1 = new MessageData
            {
                MessageId = 2,
                ReplyThreadId = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Star
            };

            recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello3", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Star, recv.Event);
            Assert.IsTrue(recv.Starred);
        }

        [Test]
        public void Receive_UpdatingMultipleMessages_OnlyTheGivenMessagesAreUpdated()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);

            Receive_StoringMultipleMessages_AllMessagesAreStoredAndReturned();

            MessageData message1 = new MessageData
            {
                MessageId = 0,
                Message = "Hello World!",
                ReplyThreadId = 0,
                Type = MessageType.Chat,
                Event = MessageEvent.Update
            };

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello World!", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Update, recv.Event);
            Assert.IsFalse(recv.Starred);

            message1 = new MessageData
            {
                MessageId = 2,
                Message = "Hello There",
                ReplyThreadId = 1,
                Type = MessageType.Chat,
                Event = MessageEvent.Update
            };

            recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello There", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Update, recv.Event);
            Assert.IsFalse(recv.Starred);
        }

        [Test]
        public void GetAllMessages_GetAllTheMessagesStoredOnTheServer_ListOfChatContextsIsReturnedWithAllTheMessages()
        {
            Receive_StarringMultipleMessages_OnlyTheGivenMessagesAreStarred();

            List<ChatContext> msgList = chatContextServer.GetAllMessages();

            ReceiveMessageData message1 = msgList[0].MsgList[0];
            Assert.AreEqual("Hello", message1.Message);
            Assert.AreEqual(MessageType.Chat, message1.Type);
            Assert.AreEqual(1, message1.SenderId);
            Assert.AreEqual(MessageEvent.Star, message1.Event);
            Assert.IsTrue(message1.Starred);

            message1 = msgList[0].MsgList[1];
            Assert.AreEqual("Hello2", message1.Message);
            Assert.AreEqual(MessageType.Chat, message1.Type);
            Assert.AreEqual(1, message1.SenderId);
            Assert.AreEqual(MessageEvent.NewMessage, message1.Event);
            Assert.IsFalse(message1.Starred);

            message1 = msgList[1].MsgList[0];
            Assert.AreEqual("Hello3", message1.Message);
            Assert.AreEqual(MessageType.Chat, message1.Type);
            Assert.AreEqual(1, message1.SenderId);
            Assert.AreEqual(MessageEvent.Star, message1.Event);
            Assert.IsTrue(message1.Starred);
        }
    }
}