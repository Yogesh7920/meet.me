using NUnit.Framework;
using Content;
using FluentAssertions;
using Networking;
using System;
using System.IO;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ChatClientTests
    {
		
        [SetUp]
        public void Setup()
        {
			
		}

        [Test]
        public void Test_Converting_SendMessageObject_To_MessageData()
        {
			Utils _util = new Utils();
			SendMessageData SampleData = _util.GetSendMessageData2();
			ChatClient conch = new ChatClient(_util.GetFakeCommunicator());
			MessageData x = conch.SendToMessage(SampleData, MessageEvent.NewMessage);

			Assert.AreEqual(x.Message, SampleData.Message);
			Assert.AreEqual(x.Event, MessageEvent.NewMessage);
			Assert.AreEqual(x.Type, SampleData.Type);
			Assert.AreEqual(x.FileData, null);
			Assert.AreEqual(x.Starred, false);
			Assert.AreEqual(x.ReplyThreadId, SampleData.ReplyThreadId);
			Assert.AreEqual(x.ReceiverIds.Length, SampleData.ReceiverIds.Length);
        }

		[Test]
		public void Test_ChatNewMessage()
		{
			Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData("Apple", new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, "Apple", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
           
			ChatClient _contentChat = new ChatClient(_util.GetFakeCommunicator());
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentChat.UserId = UserId;
            _contentChat.Communicator = _fakeCommunicator;

            _contentChat.ChatNewMessage(SampleData);

            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, SampleMsgData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, SampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData, SampleMsgData.FileData);
                Assert.AreEqual(receivedMessage.Starred, SampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, SampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, SampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }
		[Test]
		public void Test_ChatNewMessage_NullString()
		{
			Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            ISerializer _serializer = new Serializer();
           
			ChatClient _contentChat = new ChatClient(_util.GetFakeCommunicator());
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentChat.UserId = UserId;
            _contentChat.Communicator = _fakeCommunicator;

            
            ArgumentException ex = Assert.Throws<ArgumentException>(() => _contentChat.ChatNewMessage(SampleData));
            Assert.AreEqual("Null Message String", ex.Message);
		}
		[Test]
		public void Test_ChatUpdate()
		{
			Utils _util = new Utils();
            int UserId = 1001;
			int MsgId = 10;
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.Update, "Banana", new int[] { }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
           
			ChatClient _contentChat = new ChatClient(_util.GetFakeCommunicator());
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentChat.UserId = UserId;
            _contentChat.Communicator = _fakeCommunicator;

            _contentChat.ChatUpdate(MsgId,"APPLE");

            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, "APPLE");
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Update);
                Assert.AreEqual(receivedMessage.Type, SampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
				Assert.AreEqual(receivedMessage.MessageId, MsgId);

            }
            else
            {
                Assert.Fail();
            }
		}
		[Test]
		public void Test_Star()
		{
			Utils _util = new Utils();
            int UserId = 1001;
			int MsgId = 10;
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.Update, "", new int[] { }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
           
			ChatClient _contentChat = new ChatClient(_util.GetFakeCommunicator());
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentChat.UserId = UserId;
            _contentChat.Communicator = _fakeCommunicator;

            _contentChat.ChatStar(MsgId);

            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Star);
                Assert.AreEqual(receivedMessage.Type, SampleMsgData.Type);
                Assert.AreEqual(receivedMessage.ReplyThreadId, SampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
				Assert.AreEqual(receivedMessage.MessageId, MsgId);

            }
            else
            {
                Assert.Fail();
            }
		}
    }
}