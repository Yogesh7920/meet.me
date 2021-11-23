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
        public void SetUp()
        {

        }

        [Test]
        public void GetInstance_ContentClientFactory_IContentClientShouldBeSingleton()
        {
            IContentClient client1 = ContentClientFactory.GetInstance();
            IContentClient client2 = ContentClientFactory.GetInstance();
            Assert.AreEqual(client1, client2);
        }

        [Test]
        public void SetUser_ContentClientFactory_UserIdOfContentClientShouldMatchWithGivenID()
        {
            int userId = 1001;
            ContentClient contentClient = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(userId);
            Assert.AreEqual(userId, contentClient.UserId);
        }

        [Test]
        public void GetInstance_ContentServerFactory_IContentServerShouldBeSingleton()
        {
            IContentServer _server1 = ContentServerFactory.GetInstance();
            IContentServer _server2 = ContentServerFactory.GetInstance();
            Assert.AreEqual(_server1, _server2);
        }

        [Test]
        public void GetUserId_GettingContentClientUserId_UserIdOfContentClientShouldMatchWithReturnedID()
        {
            int UserId = 1001;
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            ContentClientFactory.SetUser(UserId);
            IContentClient _iContentClient = ContentClientFactory.GetInstance();
            int RcvUserId = _iContentClient.GetUserId();
            Assert.AreEqual(RcvUserId, _contentClient.UserId);
        }

        [Test]
        public void CSend_InvalidTypeSend_ShouldThrowException()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: (MessageType)2);
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            _contentClient.UserId = UserId;
            IContentClient _iContentClient = _contentClient;
            ArgumentException ex = Assert.Throws<ArgumentException>(() => _iContentClient.CSend(SampleData));
            Assert.AreEqual("Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File", ex.Message);
        }

        [Test]
        public void CSend_ChatSendingHiMsg_SerializedStringShouldMatchInputMsg()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData("Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CSend(SampleData);

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
        public void CSend_ChatSendingMsgWithNewline_SerializedStringShouldMatchInputMsg()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { 1002 }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CSend(SampleData);

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
        public void CSend_ChatSendingMsgWithNullString_SerializedStringShouldMatchInputMsg()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData(null, new int[] { 1002 }, type: MessageType.Chat);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, null, new int[] { 1002 }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CSend(SampleData);

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
        public void CSend_ChatSendingHiMsgWithBroadcast_SerializedStringShouldMatchInputMsg()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            SendMessageData SampleData = _util.GenerateChatSendMsgData("Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, "Hello, How are you?\n I am fine", new int[] { }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CSend(SampleData);

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
        public void CSend_FileSendingValidFilePathToServer_SerializedStringShouldMatchFileData()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var Filedata = new SendFileData(FilePath);
            SendMessageData SampleData = _util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;
            _iContentClient.CSend(SampleData);
            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

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
            Utils _util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FilePath = ".\\InvalidFile.pdf";
            SendMessageData SampleData = _util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(() => _iContentClient.CSend(SampleData));
            Assert.AreEqual("File " + FilePath + " not found", ex.Message);
        }

        [Test]
        public void CMarkStar_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperMarkstarReq()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            int MsgId = 10;
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.Star, "", new int[] { }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CMarkStar(MsgId);

            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Star);
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
        public void CUpdate_ClientShouldSendProperRequestToServer_SerializedStrMustHaveProperUpdateReq()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            int MsgId = 12;
            string UpdateChat = "Hi, This is updated msg.";
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.Update, UpdateChat , new int[] { }, type: MessageType.Chat);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            _iContentClient.CUpdateChat(MsgId, UpdateChat);

            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                Assert.AreEqual(receivedMessage.Event, MessageEvent.Update);
                Assert.AreEqual(receivedMessage.Type, SampleMsgData.Type);
                Assert.AreEqual(receivedMessage.SenderId, UserId);
                Assert.AreEqual(receivedMessage.MessageId, MsgId);
                Assert.AreEqual(receivedMessage.Message, UpdateChat);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CDownload_SendingDownloadReqToServerWithInvalidMsgId_ShouldThrowArgumentException()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            int MsgId = 100;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.Download, SavePath, new int[] { }, type: MessageType.File);

            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;

            ArgumentException ex = Assert.Throws<ArgumentException>(() => _iContentClient.CDownload(MsgId, SavePath));
            Assert.AreEqual("Message with given message ID not found", ex.Message);
        }

        // CDownload valid file remaining, not file download remaining
        [Test]
        public void CDownload_NonFileType_ShouldThrowArgumentException()
        {
            int MsgId = 10;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize = _util.GenerateNewMessageData("Hello", ReplyThreadId: 10, MessageId: MsgId);
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize));
            System.Threading.Thread.Sleep(10);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => _iContentClient.CDownload(MsgId, SavePath));
            Assert.AreEqual("Message requested for download is not a file type message", ex.Message);
        }

        [Test]
        public void CDownload_ValidFileMsgExistInDataBase_ShouldSendProperReqToServer()
        {
            Utils _util = new Utils();
            int UserId = 1001;
            int MsgId = 11;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            var Filedata = new SendFileData(FilePath);
            MessageData SampleMsgDataSend = _util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);
            SampleMsgDataSend.MessageId = MsgId;
            SampleMsgDataSend.SenderId = UserId;
            string SavePath = CurrentDirectory + "\\SavedTestFile.pdf";
            MessageData SampleMsgDataDownload = _util.GenerateChatMessageData(MessageEvent.Download, SavePath, new int[] { }, type: MessageType.File);
            SampleMsgDataDownload.MessageId = MsgId;
            SampleMsgDataDownload.SenderId = UserId;
            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            _contentClient.UserId = UserId;
            _contentClient.Communicator = _fakeCommunicator;
            IContentClient _iContentClient = _contentClient;
            _fakeCommunicator.Notify(_serializer.Serialize(SampleMsgDataSend));
            System.Threading.Thread.Sleep(10);
            _iContentClient.CDownload(MsgId, SavePath);
            System.Threading.Thread.Sleep(10);
            var sendSerializedMsg = _fakeCommunicator.GetSentData();
            var deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);

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
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            // Building receiveMessageData to notify to subscribers
            ReceiveMessageData _receivedData = new ReceiveMessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            _receivedData.MessageId = 2;
            // Notifying to subscribers
            _contentClient.Notify(_receivedData);
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, Msg);
        }

        [Test]
        public void CSubscribe_MultipleSubcribingToContentClient_SubscriberShouldGetMsgOnNotify()
        {
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener1 = new FakeContentListener();
            IContentListener _iFakeListener1 = _fakeListener1;
            FakeContentListener _fakeListener2 = new FakeContentListener();
            IContentListener _iFakeListener2 = _fakeListener2;
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener1);
            _iContentClient.CSubscribe(_iFakeListener2);
            // Building receiveMessageData to notify to subscribers
            ReceiveMessageData _receivedData = new ReceiveMessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            // Notifying to subscribers
            _contentClient.Notify(_receivedData);
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
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize = _util.GenerateNewMessageData("Hello");
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize));
            System.Threading.Thread.Sleep(10);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, DataToSerialize.Message);
        }

        [Test]
        public void OnDataReceived_MultipleNewMessage_SameMsgShouldReceivedToSubscriber()
        {
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize1 = _util.GenerateNewMessageData("Hello");
            MessageData DataToSerialize2 = _util.GenerateNewMessageData("Hi");
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize1));
            System.Threading.Thread.Sleep(10);
            // Msg1
            ReceiveMessageData _listenedData1 = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData1.Message, DataToSerialize1.Message);
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize2));
            System.Threading.Thread.Sleep(10);
            // Msg2
            ReceiveMessageData _listenedData2 = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData2.Message, DataToSerialize2.Message);
        }

        [Test]
        public void OnDataReceived_DownloadMessage_FileShouldBeSaved()
        {            
            Utils _util = new Utils();
            int UserId = 1001;
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string[] path = CurrentDirectory.Split(new string[] { "\\Testing" }, StringSplitOptions.None);
            string FilePath = path[0] + "\\Testing\\Content\\Test_File.pdf";
            string SavePath = path[0] + "\\Testing\\Content\\Save_";
            var Filedata = new SendFileData(FilePath);
            SendMessageData SampleData = _util.GenerateChatSendMsgData(FilePath, new int[] { }, type: MessageType.File);
            MessageData SampleMsgData = _util.GenerateChatMessageData(MessageEvent.NewMessage, FilePath, new int[] { }, type: MessageType.File);
            ISerializer _serializer = new Serializer();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            _contentClient.Communicator = _fakeCommunicator;
            _contentClient.UserId = UserId;
            IContentClient _iContentClient = _contentClient;
            _iContentClient.CSend(SampleData);
            string sendSerializedMsg = _fakeCommunicator.GetSentData();
            MessageData deserialized = _serializer.Deserialize<MessageData>(sendSerializedMsg);
            deserialized.Message = SavePath;
            deserialized.Event = MessageEvent.Download;
            _fakeCommunicator.Notify(_serializer.Serialize(deserialized));
            System.Threading.Thread.Sleep(50);
            if (File.Exists(SavePath+deserialized.FileData.fileName))
            {
                File.Delete(SavePath + deserialized.FileData.fileName);
                Assert.Pass();
            }
        }

        [Test]
        public void OnDataReceived_ListChatContext_SameChatContextsShouldReceivedToSubscriber()
        {
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            ReceiveMessageData DataToSerialize1 = _util.GenerateNewReceiveMessageData("Hello", MessageId: 1, ReplyThreadId: 1);
            ReceiveMessageData DataToSerialize2 = _util.GenerateNewReceiveMessageData("Hi", MessageId: 2, ReplyThreadId: 2);
            ReceiveMessageData DataToSerialize3 = _util.GenerateNewReceiveMessageData("How are you? I am fine!", MessageId: 2, ReplyThreadId: 1);
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
            string SerializedStr = _serializer.Serialize(ListCContext);
            _fakeCommunicator.Notify(SerializedStr);
            
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
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            MessageData DataToSerialize1 = _util.GenerateNewMessageData("Hello", MessageId: 1, ReplyThreadId: 1);
            MessageData DataToSerialize2 = _util.GenerateNewMessageData("Hi", MessageId: 2, ReplyThreadId: 2);
            MessageData DataToSerialize3 = _util.GenerateNewMessageData("How are you? I am fine!", MessageId: 2, ReplyThreadId: 1);
            ChatContext ChatList1 = new ChatContext();
            ChatList1.ThreadId = 1;
            ChatList1.MsgList.Add(DataToSerialize1);
            ChatList1.MsgList.Add(DataToSerialize3);
            ChatContext ChatList2 = new ChatContext();
            ChatList2.MsgList.Add(DataToSerialize2);
            ChatList2.ThreadId = 2;
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize1));
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize2));
            _fakeCommunicator.Notify(_serializer.Serialize(DataToSerialize3));
            ChatContext ChatsOnContext1 = _iContentClient.CGetThread(1);
            CompareChatContext(ChatList1, ChatsOnContext1);
            ChatContext ChatsOnContext2 = _iContentClient.CGetThread(2);
            CompareChatContext(ChatList2, ChatsOnContext2);
        }

        [Test]
        public void CGetThread_InvalidThreadIdGiven_ShouldThrowException()
        {
            Utils _util = new Utils();
            ContentClient _contentClient = ContentClientFactory.GetInstance() as ContentClient;
            IContentClient _iContentClient = _contentClient;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeListener = _fakeListener;
            FakeCommunicator _fakeCommunicator = _util.GetFakeCommunicator();
            INotificationHandler _notificationHandler = new ContentClientNotificationHandler(_contentClient);
            ISerializer _serializer = new Serializer();
            // Subscribing to communicator
            _fakeCommunicator.Subscribe("Content", _notificationHandler);
            // Subscribing to content client
            _iContentClient.CSubscribe(_iFakeListener);
            ArgumentException ex = Assert.Throws<ArgumentException>(() => _iContentClient.CGetThread(2));
            Assert.AreEqual("Thread with requested thread ID does not exist", ex.Message);
        }

        [Test]
        public void SSubscribe_SubcribingToContentServer_SubscriberShouldGetMsgOnNotify()
        {
            Utils _util = new Utils();
            ContentServer _contentServer = ContentServerFactory.GetInstance() as ContentServer;
            IContentServer _iContentServer = _contentServer;
            FakeContentListener _fakeListener = new FakeContentListener();
            IContentListener _iFakeServerListener = _fakeListener;
            ISerializer _serializer = new Serializer();
            // Subscribing to content client
            _iContentServer.SSubscribe(_iFakeServerListener);
            // Building receiveMessageData to notify to subscribers
            MessageData _receivedData = new MessageData();
            string Msg = "hello";  // data will have msg hello
            _receivedData.Message = Msg;
            _receivedData.ReplyThreadId = -1;
            _receivedData.Type = MessageType.Chat;
            _receivedData.Event = MessageEvent.NewMessage;
            // Notifying to subscribers
            _contentServer.Receive(_serializer.Serialize(_receivedData));
            System.Threading.Thread.Sleep(50);
            // Fetching listened data from listener
            ReceiveMessageData _listenedData = _fakeListener.GetOnMessageData();
            Assert.AreEqual(_listenedData.Message, Msg);
        }

        [Test]
        public void ReceiveNewMsgServer_ReceivingRequestFromClient_ProperReplyShouldSend()
        {

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
    }
}
