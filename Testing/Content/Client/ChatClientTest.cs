/// <author>Vishesh Munjal</author>
/// <created>20/11/2021</created>
/// <summary>
/// This file contains Test for chatClient
/// </summary>
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
		/// <summary>
		/// This Test checks the conversion function SendToMessage of the ChatClient 
		/// </summary>
        [Test]
        public void Test_Converting_SendMessageObject_To_MessageData()
        {
			Utils util = new Utils();
			SendMessageData sampleData = util.GenerateChatSendMsgData("Apple", new int[] { 1002 }, type: MessageType.Chat);
			ChatClient conch = new ChatClient(util.GetFakeCommunicator());
			MessageData x = conch.SendToMessage(sampleData, MessageEvent.NewMessage);

			Assert.AreEqual(x.Message, sampleData.Message);
			Assert.AreEqual(x.Event, MessageEvent.NewMessage);
			Assert.AreEqual(x.Type, sampleData.Type);
			Assert.AreEqual(x.FileData, null);
			Assert.AreEqual(x.Starred, false);
			Assert.AreEqual(x.ReplyThreadId, sampleData.ReplyThreadId);
			Assert.AreEqual(x.ReceiverIds.Length, sampleData.ReceiverIds.Length);
        }
		/// <summary>
		/// This Test checks the ChatNewMessage  of the ChatClient by using a Message "Apple" 
		/// </summary>
		[Test]
		public void Test_ChatNewMessage()
		{
			Utils util = new Utils();
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Apple", new int[] { 1002 }, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Apple", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
           
			ChatClient contentChat = new ChatClient(util.GetFakeCommunicator());
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatNewMessage(sampleData);

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
		/// This Test checks the ChatNewMessage function of the ChatClient in case of an Empty String.
		/// </summary>
		[Test]
		public void Test_ChatNewMessage_EmptyString()
		{
			Utils util = new Utils();
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("", new int[] { 1002 }, type: MessageType.Chat);
           
			ChatClient contentChat = new ChatClient(util.GetFakeCommunicator());
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            
            ArgumentException ex = Assert.Throws<ArgumentException>(() => contentChat.ChatNewMessage(sampleData));
			bool contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
		}
		/// <summary>
		/// This Test checks the ChatNewMessage function of the ChatClient in case of an Null String.
		/// </summary>
		[Test]
		public void Test_ChatNewMessage_NullString()
		{
			Utils util = new Utils();
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            
           
			ChatClient contentChat = new ChatClient(util.GetFakeCommunicator());
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            
            ArgumentException ex = Assert.Throws<ArgumentException>(() => contentChat.ChatNewMessage(sampleData));
			bool contains = ex.Message.IndexOf("Invalid Message", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
		}
		/// <summary>
		/// This Test checks the ChatUpdate function of the ChatClient using a chat and updating it.
		/// </summary>
		[Test]
		public void Test_ChatUpdate()
		{
			Utils util = new Utils();
            int userId = 1001;
			int msgId = 10;
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.Update, "Banana", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
           
			ChatClient contentChat = new ChatClient(util.GetFakeCommunicator());
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatUpdate(msgId,"APPLE");

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, "APPLE");
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Update);
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
		/// This Test checks the ChatStar function of the ChatClient.
		/// </summary>
		[Test]
		public void Test_Star()
		{
			Utils util = new Utils();
            int userId = 1001;
			int msgId = 10;
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.Update, "", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
           
			ChatClient contentChat = new ChatClient(util.GetFakeCommunicator());
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentChat.UserId = userId;
            contentChat.Communicator = fakeCommunicator;

            contentChat.ChatStar(msgId);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Star);
                Assert.AreEqual(receivedMessage.Type, sampleMsgData.Type);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, userId);
				Assert.AreEqual(receivedMessage.MessageId, msgId);

            }
            else
            {
                Assert.Fail();
            }
		}
    }
}



