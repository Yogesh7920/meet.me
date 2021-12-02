/// <author>Yuvraj Raghuvanshi</author>
/// <created>12/11/2021</created>

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Content;
using Networking;
using NUnit.Framework;

namespace Testing.Content
{
    [TestFixture]
    public class ContentClientTests
    {
        [SetUp]
        public void Setup()
        {
            fakeComm = new FakeCommunicator();
            listener = new FakeContentListener();
            cClient = ContentClientFactory.GetInstance() as ContentClient;
            cClient.UserId = userId;
            cClient.Communicator = fakeComm;

            cClient.CSubscribe(listener);

            // simulate a file and a chat message being received from the server so we can test certain things
            // create a chat message that will act as if it came from the server
            chatmsg = new MessageData();
            chatmsg.Event = MessageEvent.NewMessage;
            chatmsg.Type = MessageType.Chat;
            chatmsg.FileData = null;
            chatmsg.Message = "Helo";
            chatmsg.MessageId = ++maxValidMsgId;
            chatmsg.ReceiverIds = new int[0];
            chatmsg.ReplyThreadId = ++maxValidThreadId;
            chatmsg.SenderId = 1;

            // using the OnReceive function to simulate messages being received on the client
            cClient.OnReceive(chatmsg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // similarly simulate a new file message being received using the OnReceive function
            filemsg = new MessageData();
            filemsg.Event = MessageEvent.NewMessage;
            filemsg.Type = MessageType.File;
            filemsg.FileData = new SendFileData(testfilepath);
            filemsg.Message = Path.GetFileName(testfilepath);
            filemsg.MessageId = ++maxValidMsgId;
            filemsg.ReceiverIds = new int[0];
            filemsg.ReplyThreadId = maxValidThreadId;
            filemsg.SenderId = 2;

            cClient.OnReceive(filemsg);
            Thread.Sleep(sleeptime);

            // also simulate a chat message sent by the client itself, i.e., its sender id is same as user id
            userchatmsg = chatmsg.Clone();
            userchatmsg.SenderId = userId;
            userchatmsg.ReplyThreadId = ++maxValidThreadId; // part of new thread because why not
            userchatmsg.MessageId = ++maxValidMsgId;
            cClient.OnReceive(userchatmsg);
            Thread.Sleep(sleeptime);
        }

        [TearDown]
        public void TearDown()
        {
            cClient.Reset();
        }

        private FakeCommunicator fakeComm;
        private FakeContentListener listener;
        private ContentClient cClient;
        private readonly Utils u;
        private readonly ISerializer serializer;
        private readonly int userId = 42;

        private readonly string testfilepath;

        // the testing thread will need to sleep for a few ms as some functions to be tested create a separate execution thread and may take some time to show the result
        private readonly int sleeptime;

        // we'll simulate a chat and a file message being received on the client from the server
        private MessageData chatmsg, userchatmsg, filemsg;
        private int maxValidMsgId; // will represent that message ids beyond this value are invalid
        private int maxValidThreadId; // represents that thread ids beyond this value are invalid

        public ContentClientTests()
        {
            maxValidMsgId = 0;
            maxValidThreadId = 0;
            sleeptime = 50; // trial and error
            u = new Utils();
            serializer = new Serializer();

            // initialize filepath with path for test file
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] {@"\Testing"}, StringSplitOptions.None)[0];
            testfilepath = path + @"\Testing\Content\Test_File.pdf";
        }

        // helper function that checks if a string contains a substring ignoring case
        public bool Contains(string original, string match)
        {
            var result = original.IndexOf(match, StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }

        // CSend Tests

        [Test]
        public void CSend_ReceiverIdsIsNull_ShouldThrowException()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.ReceiverIds = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));

            // make sure exception contains the phrase receiver id to indicate what's wrong
            Assert.That(Contains(e.Message, "receiver id"));
        }

        [Test]
        public void CSend_UnknownMessageType_ShouldThrowException()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.ReceiverIds = new int[0];

            // get an invalid enum value, take maximum + 1
            // this assumes that the value isn't maximum possible value of int
            msg.Type = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().Max();
            msg.Type = msg.Type + 1;

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            Assert.That(Contains(e.Message, "type"));
        }

        [Test]
        public void CSend_NullStringInChat_ShouldThrowException()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.Message = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
        }

        [Test]
        public void CSend_NewMessageWithInvalidThreadId_ShouldThrowException()
        {
            var msg = u.GenerateChatSendMsgData(replyId: maxValidThreadId + 1);

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void CSend_NewMessage_ReplyToNonExistentMessage_ShouldThrowException()
        {
            // create a message that replies to a non existent message
            var msg = new SendMessageData();
            msg.Message = "Hello there!";
            msg.ReplyMsgId = ++maxValidMsgId; // no message with this id exists
            msg.Type = MessageType.Chat;
            msg.ReceiverIds = new int[0];

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            Assert.That(Contains(e.Message, "reply message id"));
        }

        /// <summary>
        ///     A reply to a message should be either part of the same thread as
        ///     that message or part of an entirely new thread
        /// </summary>
        [Test]
        public void CSend_NewMessage_ReplyInDifferentThread_ShouldThrowException()
        {
            // create a message that replies to a non existent message
            var msg = new SendMessageData();
            msg.Message = "Hello there!";
            msg.ReplyMsgId = chatmsg.MessageId;
            msg.Type = MessageType.Chat;
            msg.ReceiverIds = new int[0];
            // thread id of an existing thread that is different than that of the replied to message
            msg.ReplyThreadId = userchatmsg.ReplyThreadId;

            var e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            // see that the exception message contains mention of both reply message id and thread id
            Assert.That(Contains(e.Message, "reply message id"));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void CSend_ValidChatMessageNotAReply_ShouldSendDataToServer()
        {
            var msg = u.GenerateChatSendMsgData();

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(msg.Message, sentMsg.Message);
            Assert.AreEqual(msg.ReplyMsgId, sentMsg.ReplyMsgId);
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        [Test]
        public void CSend_ValidReplyExistingThread_ShouldSendDataToServer()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.ReplyMsgId = chatmsg.MessageId;
            msg.ReplyThreadId = chatmsg.ReplyThreadId; // replying to chatmsg in its thread

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(msg.Message, sentMsg.Message);
            Assert.AreEqual(msg.ReplyMsgId, sentMsg.ReplyMsgId);
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        [Test]
        public void CSend_ValidReplyNewThread_ShouldSendDataToServer()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.ReplyMsgId = chatmsg.MessageId;
            msg.ReplyThreadId = -1;

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(msg.Message, sentMsg.Message);
            Assert.AreEqual(msg.ReplyMsgId, sentMsg.ReplyMsgId);
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        [Test]
        public void CSend_ValidReplyNotBroadcast_ShouldSendDataToServer()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.ReplyMsgId = chatmsg.MessageId;
            msg.ReceiverIds = new[] {1, 2, 5}; // reply only to these users
            // doesn't really matter if users exist or not, that's not content module's job to verify

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(msg.Message, sentMsg.Message);
            Assert.AreEqual(msg.ReplyMsgId, sentMsg.ReplyMsgId);
            // receiver ids should match since the precursor message was broadcast
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        [Test]
        public void CSend_InvalidFilePath_ShouldThrowException()
        {
            var msg = u.GenerateChatSendMsgData(type: MessageType.File);
            // give an invalid file path
            msg.Message = "";

            _ = Assert.Throws<FileNotFoundException>(() => cClient.CSend(msg));
        }

        [Test]
        public void CSend_ValidFileMessage_ShouldSendDataToServer()
        {
            var msg = u.GenerateChatSendMsgData();
            msg.Type = MessageType.File;
            msg.ReceiverIds = new int[0];
            msg.Message = testfilepath;

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            // read file data to be able to validate sent data
            var filedata = new SendFileData(testfilepath);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(Path.GetFileName(msg.Message), sentMsg.Message);
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.AreEqual(filedata.fileContent, sentMsg.FileData.fileContent);
            Assert.AreEqual(filedata.fileName, sentMsg.FileData.fileName);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        // CDownload tests

        [Test]
        public void CDownload_InvalidMessageId_ShouldThrowException()
        {
            // give an invalid message id
            var messageId = maxValidMsgId + 1;
            // savepath doesn't matter in this case
            var savepath = Path.GetRandomFileName();

            var e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "not found"));
        }

        [Test]
        public void CDownload_AttemptToDownloadChatMessage_ShouldThrowException()
        {
            // give message id of chat message we simulated receiving
            var messageId = chatmsg.MessageId;
            // savepath shouldn't matter
            var savepath = Path.GetRandomFileName();

            var e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "not a file"));
        }

        [Test]
        public void CDownload_InvalidPath_ShouldThrowException()
        {
            // give id of file we simulated receiving in SetUp method
            var messageId = filemsg.MessageId;
            // give a save path that isn't valid
            var savepath = "";

            var e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "path"));
        }

        [Test]
        public void CDownload_ValidDownload_ShouldSendDataToServer()
        {
            // use the correct message id
            var messageId = filemsg.MessageId;
            // a path that is writable (assuming directory with test file is writable)
            var savepath = Path.GetDirectoryName(testfilepath) + Path.GetRandomFileName();

            // request download and verify the data sent to the server (fake communicator) in this case
            cClient.CDownload(messageId, savepath);

            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            // verify fields of sentMsg
            Assert.AreEqual(sentMsg.Event, MessageEvent.Download);
            Assert.AreEqual(sentMsg.MessageId, messageId);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
            Assert.AreEqual(sentMsg.Message, savepath);
        }

        // CMarkStar tests

        [Test]
        public void CMarkStar_InvalidMessageId_ShouldThrowException()
        {
            var messageid = maxValidMsgId + 1;
            var e = Assert.Throws<ArgumentException>(() => cClient.CMarkStar(messageid));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void CMarkStar_StarringFileMessage_ShouldThrowException()
        {
            var messageid = filemsg.MessageId;
            var e = Assert.Throws<ArgumentException>(() => cClient.CMarkStar(messageid));
            Assert.That(Contains(e.Message, "not chat"));
        }

        [Test]
        public void CMarkStar_StarringMessage_ShouldSendDataToServer()
        {
            var messageid = chatmsg.MessageId;
            cClient.CMarkStar(messageid);

            // validate the captured data sent to the server
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(MessageEvent.Star, sentMsg.Event);
            Assert.AreEqual(messageid, sentMsg.MessageId);
            Assert.AreEqual(chatmsg.ReplyThreadId, sentMsg.ReplyThreadId);
        }

        // CUpdateChat tests
        [Test]
        public void CUpdateChat_InvalidMessageId_ShouldThrowException()
        {
            var messageid = maxValidMsgId + 1;
            var newMessage = "loremipsum";

            var e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void CUpdateChat_NonSenderUpdates_ShouldThrowException()
        {
            var messageid = chatmsg.MessageId;
            var newMessage = "loremipsum";

            var e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "sender"));
        }

        [Test]
        public void CUpdateChat_UpdatingFileMessge_ShouldThrowException()
        {
            var messageid = filemsg.MessageId;
            var newMessage = "loremipsum";

            var e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "not chat"));
        }

        [Test]
        public void CUpdateChat_NullMessage_ShouldThrowException()
        {
            var messageid = userchatmsg.MessageId;
            string newMessage = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "invalid message"));
        }

        [Test]
        public void CUpdateChat_EmptyMessage_ShouldThrowException()
        {
            var messageid = userchatmsg.MessageId;
            var newMessage = "";

            var e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "invalid message"));
        }

        [Test]
        public void CUpdateChat_ValidUpdate_ShouldSendDataToServer()
        {
            var messageid = userchatmsg.MessageId;
            var newMessage = "loremipsum";

            cClient.CUpdateChat(messageid, newMessage);

            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            // validate necessary fields
            Assert.AreEqual(MessageEvent.Update, sentMsg.Event);
            Assert.AreEqual(messageid, sentMsg.MessageId);
            Assert.AreEqual(userchatmsg.ReplyThreadId, sentMsg.ReplyThreadId);
            Assert.AreEqual(newMessage, sentMsg.Message);
        }

        // CSubscribe tests

        [Test]
        public void CSubscribe_NullArgument_ShouldThrowException()
        {
            IContentListener subscriber = null;
            var e = Assert.Throws<ArgumentException>(() => cClient.CSubscribe(subscriber));
            Assert.That(Contains(e.Message, "null"));
        }

        // CGetThread tests

        [Test]
        public void CGetThread_InvalidThreadId_ShouldThrowException()
        {
            var threadId = maxValidThreadId + 1;
            var e = Assert.Throws<ArgumentException>(() => cClient.CGetThread(threadId));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void CGetThread_ValidThreadId_ShouldReturnThread()
        {
            // choose the thread which contains chatmsg and filemsg
            var threadId = chatmsg.ReplyThreadId;
            var context = cClient.CGetThread(threadId);

            Assert.AreEqual(context.NumOfMessages, 2);
            Assert.AreEqual(context.ThreadId, threadId);

            // look for the messages that are supposed to be in the thread
            AssertAreEqualDeep(chatmsg, context.MsgList.Find(x => x.MessageId == chatmsg.MessageId));
            AssertAreEqualDeep(filemsg, context.MsgList.Find(x => x.MessageId == filemsg.MessageId));
        }

        // OnReceive(List<ChatContext>) tests

        [Test]
        public void OnReceive_ValidContextList_ShouldSetAllMessagesAndInformSubscribers()
        {
            // create a list of chat contexts
            var contexts = cClient.AllMessages;

            // modify contexts by deleting first element
            contexts.RemoveAt(0);

            // simulate the client receiving them
            cClient.OnReceive(contexts);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // check if all messages field is set
            var newContexts = cClient.AllMessages;

            Assert.AreEqual(contexts.Count, newContexts.Count);
            for (var i = 0; i < contexts.Count; i++) AssertAreEqualDeep(contexts[i], newContexts[i]);

            // check if subscribers are informed
            var rcvdContexts = listener.GetOnAllMessagesData();

            Assert.AreEqual(contexts.Count, rcvdContexts.Count);
            for (var i = 0; i < contexts.Count; i++) AssertAreEqualDeep(contexts[i], rcvdContexts[i]);
        }

        [Test]
        public void OnReceive_ValidContextList_ShouldBeAbleToReplyToMessages()
        {
            // create a list of chat contexts
            var contexts = cClient.AllMessages;

            // simulate the process of a new client joining and receiving all these messages
            // reset and resubscribe
            cClient.Reset();
            cClient.UserId = userId;
            cClient.Communicator = fakeComm;

            cClient.CSubscribe(listener);

            // simulate receiving all messages from the server
            cClient.OnReceive(contexts);

            // try to reply to a message from the messages received
            var reply = new SendMessageData();
            reply.Message = "Hello";
            reply.Type = MessageType.Chat;
            reply.ReplyMsgId = chatmsg.ReplyMsgId;
            reply.ReceiverIds = new int[0];

            // use CSend to send reply
            cClient.CSend(reply);

            // capture sent data using fake communicator
            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(reply.Type, sentMsg.Type);
            Assert.AreEqual(reply.Message, sentMsg.Message);
            Assert.AreEqual(reply.ReplyMsgId, sentMsg.ReplyMsgId);
            Assert.AreEqual(reply.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(MessageEvent.NewMessage, sentMsg.Event);
            Assert.AreEqual(false, sentMsg.Starred);
            Assert.AreEqual(cClient.UserId, sentMsg.SenderId);
        }

        [Test]
        public void OnReceive_ValidContextList_ShouldBeAbleToDownloadFiles()
        {
            // create a list of chat contexts
            var contexts = cClient.AllMessages;

            // simulate the process of a new client joining and receiving all these messages
            // reset and resubscribe
            cClient.Reset();
            cClient.UserId = userId;
            cClient.Communicator = fakeComm;

            cClient.CSubscribe(listener);

            // simulate receiving all messages from the server
            cClient.OnReceive(contexts);

            // try to download a file from the messages
            var messageId = filemsg.MessageId;
            // a path that is writable (assuming directory with test file is writable)
            var savepath = Path.GetDirectoryName(testfilepath) + Path.GetRandomFileName();

            // request download and verify the data sent to the server (fake communicator) in this case
            cClient.CDownload(messageId, savepath);

            var sentData = fakeComm.GetSentData();
            var sentMsg = serializer.Deserialize<MessageData>(sentData);

            // verify fields of sentMsg
            Assert.AreEqual(MessageEvent.Download, sentMsg.Event);
            Assert.AreEqual(messageId, sentMsg.MessageId);
            Assert.AreEqual(cClient.UserId, sentMsg.SenderId);
            Assert.AreEqual(savepath, sentMsg.Message);
        }

        // OnReceive(MessageData) tests

        [Test]
        public void OnReceive_NewMessage_InvalidThreadId_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = -1; // invalid thread id
            msg.Message = "Hello";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void OnReceive_NewMessage_DuplicateMessageIdExistingThread_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = chatmsg.MessageId; // reuse a message id
            msg.ReplyThreadId = maxValidThreadId; // reuse a thread id as well
            msg.Message = "Hello";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_NewMessage_DuplicateMessageIdNewThread_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = chatmsg.MessageId; // reuse a message id
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = "Hello";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_NewMessage_EmptyMessageString_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = "";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message string"));
        }

        [Test]
        public void OnReceive_NewMessage_NullMessageString_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message string"));
        }

        [Test]
        public void OnReceive_NewMessage_InvalidReplyMsgId_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "reply message id"));
        }

        [Test]
        public void OnReceive_NewMessage_ReplyToNonExistentMessage_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = ++maxValidMsgId; // reply to a message that doesn't exist
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "reply message id"));
        }

        [Test]
        public void OnReceive_NewMessage_ReplyInInvalidThread_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = chatmsg.MessageId; // reply to existing message
            msg.ReplyThreadId = userchatmsg.ReplyThreadId; // but be part of a different existing thread
            msg.Message = null;

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "reply message id"));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void OnReceive_ValidNewMessageNotReply_NewThread_ShouldStoreMessageAndInformSubscribers()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = -1;
            msg.ReplyThreadId = ++maxValidThreadId; // new message thread
            msg.Message = "Hello";
            msg.SenderId = 42; // doesn't really matter

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message has been stored
            var msgContext = cClient.CGetThread(msg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(msg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            var recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_ValidNewMessageNotReply_ExistingThread_ShouldStoreMessageAndInformSubscribers()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.File; // shouldn't matter if file or chat
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = -1;
            msg.ReplyThreadId = chatmsg.ReplyThreadId; // existing message thread id
            msg.Message = "Hello";
            msg.SenderId = 401; // doesn't really matter

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message has been stored
            var msgContext = cClient.CGetThread(msg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(msg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            var recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_ValidNewReplyMessage_ExistingThread_ShouldStoreMessageAndInformSubscribers()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = chatmsg.ReplyMsgId;
            msg.ReplyThreadId = chatmsg.ReplyThreadId; // same thread id as the message replied to
            msg.Message = "Hello";
            msg.SenderId = 69; // doesn't really matter

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message has been stored
            var msgContext = cClient.CGetThread(msg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(msg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            var recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_ValidNewReplyMessage_NewThread_ShouldStoreMessageAndInformSubscribers()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyMsgId = chatmsg.ReplyMsgId;
            msg.ReplyThreadId = ++maxValidMsgId; // new thread
            msg.Message = "Hello";
            msg.SenderId = 420; // doesn't really matter

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message has been stored
            var msgContext = cClient.CGetThread(msg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(msg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            var recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_MessageUpdate_InvalidMessageId_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = ++maxValidMsgId; // try to update a message that doesn't exist
            msg.Message = "New message";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_MessageUpdate_FileMessage_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = filemsg.MessageId; // try to update a message that corresponds to a file
            msg.Message = "New message";

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
        }

        [Test]
        public void OnReceive_MessageUpdate_ValidUpdate_ShouldUpdateAndInformSubscribers()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = chatmsg.MessageId;
            msg.Message = "New message";

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message has been updated
            var msgContext = cClient.CGetThread(chatmsg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(chatmsg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            Assert.AreEqual(storedMsg.Message, msg.Message);

            // validate that the subscribers have also received notification for update
            var recvdMsg = listener.GetOnMessageData();
            Assert.AreEqual(MessageEvent.Update, recvdMsg.Event);
            Assert.AreEqual(chatmsg.MessageId, recvdMsg.MessageId);
            Assert.AreEqual(msg.Message, recvdMsg.Message);
        }

        [Test]
        public void OnReceive_MessageStar_InvalidMessageId_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.MessageId = ++maxValidMsgId; // try to star a message that doesn't exist

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_MessageStar_FileMessage_ShouldThrowException()
        {
            var msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.MessageId = filemsg.MessageId; // try to star a message that corresponds to a file

            var e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
        }

        [Test]
        public void OnReceive_MessageStar_Valid_ShouldStarAndInformSubscribers()
        {
            // we'll star chatmsg 
            var msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.MessageId = chatmsg.MessageId;

            var starStatus = chatmsg.Starred;

            cClient.OnReceive(msg);
            // sleep for a few ms as the subscriber may take a few ms to notify the client because it creates a new thread for each notification
            Thread.Sleep(sleeptime);

            // validate that the message's star status has been toggled
            var msgContext = cClient.CGetThread(chatmsg.ReplyThreadId);
            var index = msgContext.RetrieveMessageIndex(chatmsg.MessageId);
            var storedMsg = msgContext.MsgList[index];
            Assert.AreEqual(storedMsg.Starred, !starStatus);

            // validate that the subscribers have also received notification for update
            var recvdMsg = listener.GetOnMessageData();
            Assert.AreEqual(recvdMsg.Event, MessageEvent.Star);
            Assert.AreEqual(recvdMsg.MessageId, chatmsg.MessageId);
        }

        // helper functions to assert deep equality of data structures that the Content module uses

        public static void AssertAreEqualDeep(SendMessageData msg1, SendMessageData msg2)
        {
            Assert.AreEqual(msg1.Message, msg2.Message);
            Assert.AreEqual(msg1.ReceiverIds, msg2.ReceiverIds);
            Assert.AreEqual(msg1.Type, msg2.Type);
            Assert.AreEqual(msg1.ReplyThreadId, msg2.ReplyThreadId);
            Assert.AreEqual(msg1.ReplyMsgId, msg2.ReplyMsgId);
        }

        public static void AssertAreEqualDeep(ReceiveMessageData msg1, ReceiveMessageData msg2)
        {
            Assert.AreEqual(msg1.Event, msg2.Event);
            Assert.AreEqual(msg1.Type, msg2.Type);
            Assert.AreEqual(msg1.MessageId, msg2.MessageId);
            Assert.AreEqual(msg1.Message, msg2.Message);
            Assert.AreEqual(msg1.ReceiverIds, msg2.ReceiverIds);
            Assert.AreEqual(msg1.ReplyThreadId, msg2.ReplyThreadId);
            Assert.AreEqual(msg1.ReplyMsgId, msg2.ReplyMsgId);
            Assert.AreEqual(msg1.SenderId, msg2.SenderId);
            Assert.AreEqual(msg1.SentTime, msg2.SentTime);
            Assert.AreEqual(msg1.Starred, msg2.Starred);
        }

        public static void AssertAreEqualDeep(MessageData msg1, MessageData msg2)
        {
            AssertAreEqualDeep(msg1, (ReceiveMessageData) msg2);
            AssertAreEqualDeep(msg1.FileData, msg2.FileData);
        }

        public static void AssertAreEqualDeep(SendFileData fd1, SendFileData fd2)
        {
            Assert.AreEqual(fd1.fileContent, fd2.fileContent);
            Assert.AreEqual(fd1.fileName, fd2.fileName);
            Assert.AreEqual(fd1.fileSize, fd2.fileSize);
        }

        public static void AssertAreEqualDeep(ChatContext context1, ChatContext context2)
        {
            Assert.AreEqual(context1.NumOfMessages, context2.NumOfMessages);
            Assert.AreEqual(context1.CreationTime, context2.CreationTime);
            Assert.AreEqual(context1.ThreadId, context2.ThreadId);
            for (var i = 0; i < context1.NumOfMessages; i++)
                AssertAreEqualDeep(context1.MsgList[i], context2.MsgList[i]);
        }
    }
}