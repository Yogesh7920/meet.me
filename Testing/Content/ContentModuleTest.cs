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
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(userId);
            Assert.AreEqual(userId, contentClient.UserId);
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
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(userId);
            IContentClient iContentClient = ContentClientFactory.GetInstance();
            int rcvUserId = iContentClient.GetUserId();
            Assert.AreEqual(rcvUserId, contentClient.UserId);
        }

        /// <summary>
        /// This test sends invalid type supported by content client, should raise exception
        /// </summary>
        [Test]
        public void CSend_InvalidTypeSend_ShouldThrowException()
        {
            Utils util = new Utils();
            int userId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: (MessageType)2);
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            contentClient.UserId = userId;
            IContentClient iContentClient = contentClient;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(sampleData));
            Assert.AreEqual("Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File", ex.Message);
        }

        /// <summary>
        /// This test will send simple message using CSend method, we will check whether client is sending proper request to server
        /// by fetching msg from communicator, deserialize it and compare fields of Messagedata, same approach for following tests.
        /// We are sending private msg to user with id 1002
        /// </summary>
        [Test]
        public void CSend_ChatSendingHiMsg_SerializedStringShouldMatchInputMsg()
        {
            Utils util = new Utils();
            int UserId = 1001;
            SendMessageData sampleData = util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

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

        [Test]
        public void CSend_ChatSendingMsgWithNewline_SerializedStringShouldMatchInputMsg()
        {
            Utils util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

            iContentClient.CSend(SampleData);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

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
        public void CSend_ChatSendingMsgWithNullString_SerializedStringShouldMatchInputMsg()
        {
            Utils util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, null, new int[] { 1002 }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CSend(SampleData));
            Assert.AreEqual("Null Message String", ex.Message);
        }

        [Test]
        public void CSend_ChatSendingHiMsgWithBroadcast_SerializedStringShouldMatchInputMsg()
        {
            Utils util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

            iContentClient.CSend(SampleData);

            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

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
        public void CSend_FileSendingValidFilePathToServer_SerializedStringShouldMatchFileData()
        {
            Utils util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var Filedata = new SendFileData(FilePath);
            SendMessageData SampleData = util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;
            iContentClient.CSend(SampleData);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Message, Filedata.fileName);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.NewMessage);
                Assert.AreEqual(receivedMessage.Type, SampleMsgData.Type);
                Assert.AreEqual(receivedMessage.FileData.fileContent, Filedata.fileContent);
                Assert.AreEqual(receivedMessage.FileData.fileSize, Filedata.fileSize);
                Assert.AreEqual(receivedMessage.FileData.fileName, Filedata.fileName);
                Assert.AreEqual(receivedMessage.Starred, SampleMsgData.Starred);
                Assert.AreEqual(receivedMessage.ReplyThreadId, SampleMsgData.ReplyThreadId);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
                Assert.AreEqual(receivedMessage.MessageId, -1);
                Assert.AreEqual(receivedMessage.ReceiverIds.Length, SampleMsgData.ReceiverIds.Length);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CSend_InValidFilePath_ShouldThrowFileNotFoundException()
        {
            Utils util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FilePath = ".\\InvalidFile.pdf";
            SendMessageData SampleData = util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() => iContentClient.CSend(SampleData));
            Assert.AreEqual("File " + FilePath + " not found", ex.Message);
        }


        /*
            Utils util = new Utils();
            int UserId = 1001;
            int MsgId = 11;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var Filedata = new SendFileData(FilePath);
            MessageData SampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);
            SampleMsgDataSend.MessageId = MsgId;
            SampleMsgDataSend.SenderId = UserId;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            MessageData SampleMsgDataDownload = util.GenerateChatMessageData(MessageEvent.Download, SavePath, new int[] { }, type: MessageType.File);
            SampleMsgDataDownload.MessageId = MsgId;
            SampleMsgDataDownload.SenderId = UserId;
            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;
            fakeCommunicator.Notify(serializer.Serialize(SampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            iContentClient.CDownload(MsgId, SavePath);
            System.Threading.Thread.Sleep(10);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
        */

        [Test]
        public void CMarkStar_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperMarkstarReq()
        {
            Utils util = new Utils();
            int userId = 1001;
            int msgId = 13;
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.Star, "", new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            SampleMsgData.MessageId = msgId;
            fakeCommunicator.Notify(serializer.Serialize(SampleMsgData));
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

        [Test]
        public void CUpdate_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperUpdateReq()
        {
            Utils util = new Utils();
            int userId = 1001;
            int msgId = 12;
            string updateChat = "Hi, This is updated msg.";
            MessageData sampleMsgData = util.GenerateChatMessageData(MessageEvent.Update, updateChat , new int[] { }, type: MessageType.Chat);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = userId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            SampleMsgData.MessageId = msgId;
            SampleMsgData.SenderId = userId;
            fakeCommunicator.Notify(serializer.Serialize(SampleMsgData));
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

        [Test]
        public void CDownload_SendingDownloadReqToServerWithInvalidMsgId_ShouldThrowArgumentException()
        {
            Utils util = new Utils();
            int UserId = 1001;
            int MsgId = 100;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.Download, SavePath, new int[] { }, type: MessageType.File);

            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;

            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CDownload(MsgId, SavePath));
            Assert.AreEqual("Message with given message ID not found", ex.Message);
        }

        // CDownload valid file remaining, not file download remaining
        [Test]
        public void CDownload_NonFileType_ShouldThrowArgumentException()
        {
            int MsgId = 10;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize = util.GenerateNewMessageData("Hello", ReplyThreadId: 10, MessageId: MsgId);
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize));
            System.Threading.Thread.Sleep(10);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CDownload(MsgId, SavePath));
            Assert.AreEqual("Message requested for download is not a file type message", ex.Message);
        }

        [Test]
        public void CDownload_ValidFileMsgExistInDataBase_ShouldSendProperReqToServer()
        {
            Utils util = new Utils();
            int UserId = 1001;
            int MsgId = 11;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var Filedata = new SendFileData(FilePath);
            MessageData SampleMsgDataSend = util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);
            SampleMsgDataSend.MessageId = MsgId;
            SampleMsgDataSend.SenderId = UserId;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            MessageData SampleMsgDataDownload = util.GenerateChatMessageData(MessageEvent.Download, SavePath, new int[] { }, type: MessageType.File);
            SampleMsgDataDownload.MessageId = MsgId;
            SampleMsgDataDownload.SenderId = UserId;
            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            contentClient.UserId = UserId;
            contentClient.Communicator = fakeCommunicator;
            IContentClient iContentClient = contentClient;
            fakeCommunicator.Notify(serializer.Serialize(SampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            iContentClient.CDownload(MsgId, SavePath);
            System.Threading.Thread.Sleep(10);
            var sendSerializedMsg = fakeCommunicator.GetSentData();
            var deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized;
                Assert.AreEqual(receivedMessage.Message, SampleMsgDataDownload.Message);
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Download);
                Assert.AreEqual(receivedMessage.Type, SampleMsgDataDownload.Type);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
                Assert.AreEqual(receivedMessage.MessageId, MsgId);
                Assert.AreEqual(receivedMessage.FileData, null);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CSubscribe_SubcribingToContentClient_SubscriberShouldGetMsgOnNotify()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            // Building receiveMessageData to notify to subscribers
            ReceiveMessageData _receivedData = new ReceiveMessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            _receivedData.MessageId = 2;
            // Notifying to subscribers
            contentClient.Notify(_receivedData);
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, Msg);
        }

        [Test]
        public void CSubscribe_MultipleSubcribingToContentClient_SubscriberShouldGetMsgOnNotify()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener1 = new FakeContentListener();
            IContentListener _iFakeListener1 = _fakeListener1;
            FakeContentListener _fakeListener2 = new FakeContentListener();
            IContentListener _iFakeListener2 = _fakeListener2;
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener1);
            iContentClient.CSubscribe(_iFakeListener2);
            // Building receiveMessageData to notify to subscribers
            ReceiveMessageData _receivedData = new ReceiveMessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            // Notifying to subscribers
            contentClient.Notify(_receivedData);
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData1 = _fakeListener1.GetOnMessageData();
            ReceiveMessageData _listenedData2 = _fakeListener2.GetOnMessageData();
            Assert.AreEqual(_listenedData1.Message, Msg);
            Assert.AreEqual(_listenedData2.Message, Msg);
        }

        [Test]
        public void OnDataReceived_NewMessage_SameMsgShouldReceivedToSubscriber()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize = util.GenerateNewMessageData("Hello");
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize));
            System.Threading.Thread.Sleep(10);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, DataToSerialize.Message);
        }

        [Test]
        public void OnDataReceived_MultipleNewMessage_SameMsgShouldReceivedToSubscriber()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize1 = util.GenerateNewMessageData("Hello");
            MessageData DataToSerialize2 = util.GenerateNewMessageData("Hi");
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize1));
            System.Threading.Thread.Sleep(10);
            // Msg1
            ReceiveMessageData _listenedData1 = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData1.Message, DataToSerialize1.Message);
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize2));
            System.Threading.Thread.Sleep(10);
            // Msg2
            ReceiveMessageData _listenedData2 = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData2.Message, DataToSerialize2.Message);
        }

        [Test]
        public void OnDataReceived_DownloadMessage_FileShouldBeSaved()
        {            
            Utils util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            string SavePath = path[0] + "\\Testing\\Content\\Save_";
            var Filedata = new SendFileData(FilePath);
            SendMessageData SampleData = util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);
            ISerializer serializer = new Serializer();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            contentClient.Communicator = fakeCommunicator;
            contentClient.UserId = UserId;
            IContentClient iContentClient = contentClient;
            iContentClient.CSend(SampleData);
            string sendSerializedMsg = fakeCommunicator.GetSentData();
            MessageData deserialized = serializer.Deserialize<MessageData>(sendSerializedMsg);
            deserialized.Message = SavePath;
            deserialized.Event = MessageEvent.Download;
            fakeCommunicator.Notify(serializer.Serialize(deserialized));
            System.Threading.Thread.Sleep(50);
            if (File.Exists(SavePath+deserialized.FileData.fileName))
            {
                File.Delete(SavePath + deserialized.FileData.fileName);
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void OnDataReceived_ListChatContext_SameChatContextsShouldReceivedToSubscriber()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            ReceiveMessageData DataToSerialize1 = util.GenerateNewReceiveMessageData("Hello", MessageId: 1, ReplyThreadId: 1);
            ReceiveMessageData DataToSerialize2 = util.GenerateNewReceiveMessageData("Hi", MessageId: 2, ReplyThreadId: 2);
            ReceiveMessageData DataToSerialize3 = util.GenerateNewReceiveMessageData("How are you? I am fine!", MessageId: 2, ReplyThreadId: 1);
            ChatContext ChatList1 = new ChatContext();
            ChatList1.ThreadId = 1;
            ChatList1.MsgList.Add(DataToSerialize1);
            ChatList1.MsgList.Add(DataToSerialize3);
            ChatContext ChatList2 = new ChatContext();
            ChatList2.MsgList.Add(DataToSerialize2);
            ChatList2.ThreadId = 2;
            List<ChatContext> ListCContext = new List<ChatContext>();
            ListCContext.Add(ChatList1);
            ListCContext.Add(ChatList2);
            string SerializedStr = serializer.Serialize(ListCContext);
            fakeCommunicator.Notify(SerializedStr);
            
            System.Threading.Thread.Sleep(10);
            List<ChatContext> _listenedData = _fakeListener.GetOnAllMessagesData();
            for(int i=0; i<_listenedData.Count; i++)
            {
                CompareChatContext(_listenedData[i], ListCContext[i]);
            }
        }

        [Test]
        public void CGetThread_ReturnsChatContextOfGivenThreadIDMultipleThreads_ShouldMatchWithConstructedChatContext()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize1 = util.GenerateNewMessageData("Hello", MessageId: 1, ReplyThreadId: 11);
            MessageData DataToSerialize2 = util.GenerateNewMessageData("Hi", MessageId: 2, ReplyThreadId: 12);
            MessageData DataToSerialize3 = util.GenerateNewMessageData("How are you? I am fine!", MessageId: 2, ReplyThreadId: 11);
            ChatContext ChatList1 = new ChatContext();
            ChatList1.ThreadId = 11;
            ChatList1.MsgList.Add(DataToSerialize1);
            ChatList1.MsgList.Add(DataToSerialize3);
            ChatContext ChatList2 = new ChatContext();
            ChatList2.MsgList.Add(DataToSerialize2);
            ChatList2.ThreadId = 12;
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize1));
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize2));
            fakeCommunicator.Notify(serializer.Serialize(DataToSerialize3));
            ChatContext ChatsOnContext1 = iContentClient.CGetThread(11);
            CompareChatContext(ChatList1, ChatsOnContext1);
            ChatContext ChatsOnContext2 = iContentClient.CGetThread(12);
            CompareChatContext(ChatList2, ChatsOnContext2);
        }

        [Test]
        public void CGetThread_InvalidThreadIdGiven_ShouldThrowException()
        {
            Utils util = new Utils();
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            FakeContentListener fakeListener = new FakeContentListener();
            IContentListener iFakeListener = fakeListener;
            FakeCommunicator fakeCommunicator = util.GetFakeCommunicator();
            INotificationHandler notificationHandler = new ContentClientNotificationHandler(contentClient);
            ISerializer serializer = new Serializer();
            // Subscribing to communicator
            fakeCommunicator.Subscribe("Content", notificationHandler);
            // Subscribing to content client
            iContentClient.CSubscribe(iFakeListener);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => iContentClient.CGetThread(2));
            Assert.AreEqual("Thread with requested thread ID does not exist", ex.Message);
        }

        [Test]
        ///<summary>
        /// Here we are testing SGetAllMessages and SSendAllMessages of server by sending it three new message
        /// starring first message, updating second message and keeping third same which will be replies in context of first message
        /// This test will check new arrival message, broadcast, private sending, starring msg for chats
        /// </summary>
        public void SGetAllMessagesAndSendAllMessages_GettingAllMsgsFromServer_ShouldMatchSentMsgsToServer()
        {
            ContentServer contentServer = ContentServerFactory.GetInstance() as ContentServer;
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator fakeCommunicator = new FakeCommunicator();
            ISerializer serializer = new Serializer();
            Utils util = new Utils();
            contentServer.Communicator = fakeCommunicator;
            MessageData receiveMsgData1 = util.GenerateNewMessageData("Hello, how are you?", SenderId: 1001, MessageId: -1, ReplyThreadId: -1);
            MessageData receiveMsgData2 = util.GenerateNewMessageData("I am fine, How aboid u?", SenderId: 1002, MessageId: -1, ReplyThreadId: -1);
            MessageData receiveMsgData3 = util.GenerateNewMessageData("I am fine", SenderId: 1003, MessageId: -1, ReplyThreadId: -1);
            contentServer.Receive(serializer.Serialize(receiveMsgData1));
            MessageData msg1 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            contentClient.OnReceive(msg1);
            TestMsgDataFieldsServer(msg1, receiveMsgData1);
            MessageData starMsg1 = msg1;
            starMsg1.Event = MessageEvent.Star;
            contentServer.Receive(serializer.Serialize(starMsg1));
            MessageData starReplyMsg1 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            contentClient.OnReceive(starReplyMsg1);
            TestMsgDataFieldsServer(msg1, starReplyMsg1);
            contentServer.Receive(serializer.Serialize(receiveMsgData2));
            MessageData msg2 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            contentClient.OnReceive(msg2);
            TestMsgDataFieldsServer(msg2, receiveMsgData2);
            MessageData updateMsg2 = msg2;
            updateMsg2.Event = MessageEvent.Update;
            updateMsg2.Message = "I am fine, How about u?";
            contentServer.Receive(serializer.Serialize(updateMsg2));
            MessageData updateReplyMsg2 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            contentClient.OnReceive(updateReplyMsg2);
            TestMsgDataFieldsServer(updateMsg2, msg2);
            Assert.AreEqual(true, starReplyMsg1.Starred);
            Assert.AreEqual(updateReplyMsg2.Message, "I am fine, How about u?");
            receiveMsgData3.ReplyThreadId = msg1.ReplyThreadId;
            contentServer.Receive(serializer.Serialize(receiveMsgData3));
            MessageData msg3 = GetMsgFromCommunicator(fakeCommunicator, serializer, true, null);
            contentClient.OnReceive(msg3);
            TestMsgDataFieldsServer(msg3, receiveMsgData3);
            ChatContext c1 = new ChatContext();
            c1.ThreadId = msg1.ReplyThreadId;
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(starReplyMsg1));
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(msg3));
            ChatContext c2 = new ChatContext();
            c2.ThreadId = msg2.ReplyThreadId;
            c2.MsgList.Add(util.MessageDataToReceiveMessageData(updateReplyMsg2));
            chatList.Add(c1);
            chatList.Add(c2);
            List<ChatContext> listReceived = contentServer.SGetAllMessages();
            contentServer.SSendAllMessagesToClient(1003);
            TestSSendAllMessagesToClient(fakeCommunicator, serializer, chatList, 1003);
            CompareChatContextList(chatList, listReceived);
        }

        [Test]
        ///<summary>
        /// Here we are testing file related functionality of server, i.e storing new file message and handling donwload request
        /// </summary>
        public void SendingAndReceivingFileServer_NewFileMessageAndDownloadRequest_FileShouldBeDownloadedOnClient()
        {
            ContentServer contentServer = ContentServerFactory.GetInstance() as ContentServer;
            FakeCommunicator fakeClientCommunicator = new FakeCommunicator();
            FakeCommunicator fakeServerCommunicator = new FakeCommunicator();
            ISerializer serializer = new Serializer();
            Utils util = new Utils();
            contentServer.Communicator = fakeServerCommunicator;
            int UserId = 1001;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            string SavePath = path[0] + "\\Testing\\Content\\Save_";
            var Filedata = new SendFileData(FilePath);
            SendMessageData SampleData = util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient iContentClient = contentClient;
            contentClient.Communicator = fakeClientCommunicator;
            contentClient.UserId = UserId;
            iContentClient.CSend(SampleData);
            string sendSerializedMsg = fakeClientCommunicator.GetSentData();
            MessageData sendNewFileData = serializer.Deserialize<MessageData>(sendSerializedMsg);
            contentServer.Receive(sendSerializedMsg);
            MessageData fileReplyMsg = GetMsgFromCommunicator(fakeServerCommunicator, serializer, true, null);
            ChatContext c1 = new ChatContext();
            c1.ThreadId = fileReplyMsg.ReplyThreadId;
            c1.MsgList.Add(util.MessageDataToReceiveMessageData(fileReplyMsg));
            chatList.Add(c1);
            contentClient.OnReceive(fileReplyMsg);
            iContentClient.CDownload(fileReplyMsg.MessageId, SavePath);
            string downloadReqMsg = fakeClientCommunicator.GetSentData();
            contentServer.Receive(downloadReqMsg);
            List<int> rcvId = new List<int>();
            rcvId.Add(UserId);
            MessageData fileReturnedData = GetMsgFromCommunicator(fakeServerCommunicator, serializer, false, rcvId);
            Assert.AreEqual(fileReturnedData.FileData.fileContent, sendNewFileData.FileData.fileContent);
            Assert.AreEqual(fileReturnedData.Message, SavePath);
            Assert.AreEqual(fileReturnedData.FileData.fileName, sendNewFileData.FileData.fileName);
            Assert.AreEqual(fileReturnedData.MessageId, fileReplyMsg.MessageId);
            contentClient.OnReceive(fileReturnedData);
            System.Threading.Thread.Sleep(50);
            if (File.Exists(SavePath + fileReturnedData.FileData.fileName))
            {
                File.Delete(SavePath + fileReturnedData.FileData.fileName);
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void SSubscribe_SubcribingToContentServer_SubscriberShouldGetMsgOnNotify()
        {
            Utils util = new Utils();
            ContentServer _contentServer = ContentServerFactory.GetInstance() as ContentServer;
            IContentServer _iContentServer = _contentServer;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeServerListener = _fakeListener;
            ISerializer serializer = new Serializer();
            // Subscribing to content client
            _iContentServer.SSubscribe(_iFakeServerListener);
            // Building receiveMessageData to notify to subscribers
            MessageData _receivedData = new MessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            _receivedData.ReplyThreadId = -1;
            _receivedData.Type = MessageType.Chat;
            _receivedData.Event = MessageEvent.NewMessage;
            _receivedData.ReceiverIds = new int[0];
            // Notifying to subscribers
            _contentServer.Receive(serializer.Serialize(_receivedData));
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, Msg);
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

        List<ChatContext> chatList = new List<ChatContext>();
    }
}
