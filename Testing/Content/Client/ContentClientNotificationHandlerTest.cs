/// <author>Vishesh Munjal</author>
/// <created>24/11/2021</created>
/// <summary>
/// This file contains the test for the notification handler of contentclient.
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
            SendMessageData sampleData = util.GenerateChatSendMsgData("Apple", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize<SendMessageData>(sampleData);

            FakeContentHandler fakeContentHandler = new FakeContentHandler();
            ContentClientNotificationHandler conClNotHan = new ContentClientNotificationHandler(fakeContentHandler);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => conClNotHan.OnDataReceived(serializedMsg));
			bool contains = ex.Message.IndexOf("Deserialized object of unknown type:", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.That(contains);
            

        }
        [Test]
        public void Testing_OnReceiveData_Case1()
        {
            Utils util = new Utils();
            MessageData sampleData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Banana", new int[] {1002}, type: MessageType.Chat);
            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize<MessageData>(sampleData);
            FakeContentHandler fakeContentHandler = new FakeContentHandler();
            ContentClientNotificationHandler conClNotHan = new ContentClientNotificationHandler(fakeContentHandler);
            conClNotHan.OnDataReceived(serializedMsg);

            var whatWeGet = fakeContentHandler.GetOnReceive();

            
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

        public void Testing_OnReceiveData_Case2()
        {
            Utils util = new Utils();
            MessageData sampleData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Banana", new int[] {1002}, type: MessageType.Chat);
            List<ChatContext> sampleList = util.getlistContext(sampleData);

            ISerializer serializer = new Serializer();
            var serializedMsg = serializer.Serialize<MessageData>(sampleData);
            FakeContentHandler fakeContentHandler = new FakeContentHandler();
            ContentClientNotificationHandler conClNotHan = new ContentClientNotificationHandler(fakeContentHandler);
            conClNotHan.OnDataReceived(serializedMsg);

            var whatWeGet = fakeContentHandler.GetNotify();

            ContentModuleTesting conModTest = new ContentModuleTesting();
			conModTest.CompareChatContextList(whatWeGet,sampleList);

        }
	
    }
}