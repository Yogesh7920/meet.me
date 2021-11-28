/// <author>Vishesh Munjal</author>
/// <created>24/11/2021</created>
/// <summary>
/// This file contains the test for the notification handler of contentclient.
/// </summary>

using System;
using System.Collections.Generic;
using Content;
using Networking;
using NUnit.Framework;

namespace Testing.Content
{
    public class ContentClientNotificationHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        ///     This Checks the Exception of notification handler in the case that the Serialized object received was
        ///     of not the desired type.
        /// </summary>
        [Test]
        public void Testing_OnReceiveData_Exception()
        {
            var util = new Utils();
            var list = new List<int>();
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize(list);
            var contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            var contentClientNotficationHandler = new ContentClientNotificationHandler(contentHandler);
            var ex = Assert.Throws<ArgumentException>(() =>
                contentClientNotficationHandler.OnDataReceived(serializedMsg));
            var contains =
                ex.Message.IndexOf("Deserialized object of unknown type:", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
        }

        /// <summary>
        ///     Testing receiving for message data type
        /// </summary>
        [Test]
        public void Testing_OnReceiveData_Case1()
        {
            var util = new Utils();
            var sampleData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Banana", new[] {1002},
                type: MessageType.Chat, replyId: 1);
            sampleData.MessageId = 4;
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize(sampleData);
            var contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            var fakeNotifier = new FakeNotifier(contentHandler);
            fakeNotifier.OnDataReceived(serializedMsg);
            var whatWeGet = fakeNotifier.GetMessageData();


            if (whatWeGet is MessageData)
            {
                var receivedMessage = whatWeGet;
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
        ///     Testing receiving for chat context list type
        /// </summary>
        [Test]
        public void Testing_OnReceiveData_Case2()
        {
            var util = new Utils();
            var sampleData1 = util.GenerateNewReceiveMessageData("Hello", ReplyThreadId: 2, MessageId: 4);
            var sampleData2 = util.GenerateNewReceiveMessageData("Hi, how are you", ReplyThreadId: 2, MessageId: 5);
            var sampleData3 =
                util.GenerateNewReceiveMessageData("When will class start?", ReplyThreadId: 3, MessageId: 6);
            var sampleList = new List<ChatContext>();
            var c1 = new ChatContext();
            c1.ThreadId = 1;
            c1.MsgList.Add(sampleData1);
            c1.MsgList.Add(sampleData2);
            var c2 = new ChatContext();
            c2.ThreadId = 2;
            c2.MsgList.Add(sampleData3);
            sampleList.Add(c1);
            sampleList.Add(c2);
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize(sampleList);
            var contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.Communicator = new FakeCommunicator();
            IContentClient contentHandler = contentClient;
            var fakeNotifier = new FakeNotifier(contentHandler);
            fakeNotifier.OnDataReceived(serializedMsg);
            var contentModuleTest = new ContentModuleTesting();
            contentModuleTest.CompareChatContextList(fakeNotifier.GetAllMessageData(), sampleList);
        }
    }
}