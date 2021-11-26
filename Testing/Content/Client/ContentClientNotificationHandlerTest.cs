/// <author>Vishesh Munjal</author>
/// <created>24/11/2021</created>
/// <summary>
/// This file contains the test for the notification handler of contentclient.
/// </summary>
using NUnit.Framework;
using Content;
using Networking;
using System;
using System.Collections.Generic;

namespace Testing.Content
{
    public class ContentClientNotificationHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }
		/// <summary>
		/// This Checks the Exception of notification handler in the case that the Serialized object received was
		/// of not the desired type.
		/// </summary>
        [Test]
        public void Testing_OnReceiveData_Exception()
        {
            Utils util = new Utils();
            List<int> list = new List<int>();
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize(list);
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            ContentClientNotificationHandler contentClientNotficationHandler = new ContentClientNotificationHandler(contentHandler);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => contentClientNotficationHandler.OnDataReceived(serializedMsg));
			bool contains = ex.Message.IndexOf("Deserialized object of unknown type:", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        /// Testing receiving for message data type
        /// </summary>
        [Test]
        public void Testing_OnReceiveData_Case1()
        {
            Utils util = new Utils();
            MessageData sampleData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Banana", new int[] { 1002 }, type: MessageType.Chat, replyId: 1);
            sampleData.MessageId = 4;
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize<MessageData>(sampleData);
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            FakeNotifier fakeNotifier = new FakeNotifier(contentHandler);
            fakeNotifier.OnDataReceived(serializedMsg);
            var whatWeGet = fakeNotifier.GetMessageData();


            if (whatWeGet is MessageData)
            {
                var receivedMessage = whatWeGet as MessageData;
                Assert.AreEqual(receivedMessage.Message, sampleData.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, sampleData.Type);
                Assert.AreEqual(receivedMessage.FileData, sampleData.FileData);
                Assert.AreEqual(receivedMessage.Starred, sampleData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, sampleData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, sampleData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Testing receiving for chat context list type
        /// </summary>
        [Test]
        public void Testing_OnReceiveData_Case2()
        {
            Utils util = new Utils();
            ReceiveMessageData sampleData1 = util.GenerateNewReceiveMessageData("Hello", ReplyThreadId:2, MessageId:4);
            ReceiveMessageData sampleData2 = util.GenerateNewReceiveMessageData("Hi, how are you", ReplyThreadId: 2, MessageId: 5);
            ReceiveMessageData sampleData3 = util.GenerateNewReceiveMessageData("When will class start?", ReplyThreadId: 3, MessageId: 6);
            List<ChatContext> sampleList = new List<ChatContext>();
            ChatContext c1 = new ChatContext();
            c1.ThreadId = 1;
            c1.MsgList.Add(sampleData1);
            c1.MsgList.Add(sampleData2);
            ChatContext c2 = new ChatContext();
            c2.ThreadId = 2;
            c2.MsgList.Add(sampleData3);
            sampleList.Add(c1);
            sampleList.Add(c2);
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize(sampleList);
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            FakeNotifier fakeNotifier = new FakeNotifier(contentHandler);
            fakeNotifier.OnDataReceived(serializedMsg);
            ContentModuleTesting contentModuleTest = new ContentModuleTesting();
			contentModuleTest.CompareChatContextList(fakeNotifier.GetAllMessageData(),sampleList);
        }
    }
}