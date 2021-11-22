using Content;
using NUnit.Framework;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ChatContextServerTests
    {
        private ChatContextServer chatContextServer;
        private ContentDatabase database;

        [SetUp]
        public void Setup()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);
        }

        [Test]
        public void NewMessageTest()
        {
            MessageData message1 = new MessageData();
            message1.Message = "Hello";
            message1.Type = MessageType.Chat;
            message1.SenderId = 1;
            message1.ReplyThreadId = 0;
            message1.Event = MessageEvent.NewMessage;

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual(message1.Message, recv.Message);
            Assert.AreEqual(message1.Type, recv.Type);
            Assert.AreEqual(message1.SenderId, recv.SenderId);
            Assert.AreEqual(message1.Event, recv.Event);
            Assert.IsFalse(recv.Starred);

            MessageData message2 = new MessageData();
            message2.Message = "Hello2";
            message2.Type = MessageType.Chat;
            message2.SenderId = 1;
            message2.ReplyThreadId = message1.ReplyThreadId;
            message2.Event = MessageEvent.NewMessage;

            recv = chatContextServer.Receive(message2);

            Assert.AreEqual(message2.Message, recv.Message);
            Assert.AreNotEqual(message2.MessageId, message1.MessageId);
            Assert.AreEqual(message2.Type, recv.Type);
            Assert.AreEqual(message2.SenderId, recv.SenderId);
            Assert.AreEqual(message2.Event, recv.Event);
            Assert.AreEqual(message2.ReplyThreadId, recv.ReplyThreadId);
            Assert.IsFalse(recv.Starred);

            MessageData message3 = new MessageData();
            message3.Message = "Hello3";
            message3.Type = MessageType.Chat;
            message3.SenderId = 1;
            message3.ReplyThreadId = -1;
            message3.Event = MessageEvent.NewMessage;

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
        public void StarTest()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);

            NewMessageTest();

            MessageData message1 = new MessageData();
            message1.MessageId = 0;
            message1.ReplyThreadId = 0;
            message1.Type = MessageType.Chat;
            message1.Event = MessageEvent.Star;

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Star, recv.Event);
            Assert.IsTrue(recv.Starred);

            message1 = new MessageData();
            message1.MessageId = 2;
            message1.ReplyThreadId = 1;
            message1.Type = MessageType.Chat;
            message1.Event = MessageEvent.Star;

            recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello3", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Star, recv.Event);
            Assert.IsTrue(recv.Starred);
        }

        [Test]
        public void UpdateTest()
        {
            database = new ContentDatabase();
            chatContextServer = new ChatContextServer(database);

            NewMessageTest();

            MessageData message1 = new MessageData();
            message1.MessageId = 0;
            message1.Message = "Hello World!";
            message1.ReplyThreadId = 0;
            message1.Type = MessageType.Chat;
            message1.Event = MessageEvent.Update;

            ReceiveMessageData recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello World!", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Update, recv.Event);
            Assert.IsFalse(recv.Starred);

            message1 = new MessageData();
            message1.MessageId = 2;
            message1.Message = "Hello There";
            message1.ReplyThreadId = 1;
            message1.Type = MessageType.Chat;
            message1.Event = MessageEvent.Update;

            recv = chatContextServer.Receive(message1);

            Assert.AreEqual("Hello There", recv.Message);
            Assert.AreEqual(MessageType.Chat, recv.Type);
            Assert.AreEqual(1, recv.SenderId);
            Assert.AreEqual(MessageEvent.Update, recv.Event);
            Assert.IsFalse(recv.Starred);
        }

        [Test]
        public void GetAllMessageTest()
        {
            StarTest();

            List<ChatContext> msgList = chatContextServer.GetAllMessages();

            ReceiveMessageData message1 = msgList[0].MsgList[0];
            Assert.AreEqual("Hello", message1.Message);
            Assert.AreEqual(MessageType.Chat, message1.Type);
            Assert.AreEqual(1, message1.SenderId);
            Assert.AreEqual(MessageEvent.Star, message1.Event);
            Assert.IsTrue(message1.Starred);

            message1 = msgList[1].MsgList[0];
            Assert.AreEqual("Hello3", message1.Message);
            Assert.AreEqual(MessageType.Chat, message1.Type);
            Assert.AreEqual(1, message1.SenderId);
            Assert.AreEqual(MessageEvent.Star, message1.Event);
            Assert.IsTrue(message1.Starred);
        }
    }
}