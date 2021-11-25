/// <author>Yuvraj Raghuvanshi</author>
/// <created>12/11/2021</created>

using NUnit.Framework;
using Content;
using Networking;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Testing.Content
{
    [TestFixture]
    public class ContentClientTests
    {
        FakeCommunicator fakeComm;
        FakeContentListener listener;
        ContentClient cClient;
        Utils u;
        ISerializer serializer;
        int userId = 42;
        string testfilepath;

        // we'll simulate a chat and a file message being received on the client from the server
        MessageData chatmsg, userchatmsg, filemsg;
        int maxValidMsgId; // will represent that message ids beyond this value are invalid
        int maxValidThreadId; // represents that thread ids beyond this value are invalid

        public ContentClientTests()
        {
            maxValidMsgId = 0;
            maxValidThreadId = 0;
            u = new Utils();
            serializer = new Serializer();

            // initialize filepath with path for test file
            string CurrentDirectory = Directory.GetCurrentDirectory() as string;
            string path = CurrentDirectory.Split(new string[] { @"\Testing" }, StringSplitOptions.None)[0];
            testfilepath = path + @"\Testing\Content\Test_File.pdf";
        }

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

            // also simulate a chat message sent by the client itself, i.e., its sender id is same as user id
            userchatmsg = chatmsg.Clone();
            userchatmsg.SenderId = userId;
            userchatmsg.ReplyThreadId = ++maxValidThreadId; // part of new thread because why not
            userchatmsg.MessageId = ++maxValidMsgId;
            cClient.OnReceive(userchatmsg);
        }

        [TearDown]
        public void TearDown()
        {
            cClient.Reset();
        }

        // helper function that checks if a string contains a substring ignoring case
        public bool Contains(string original, string match)
        {
            bool result = original.IndexOf(match, StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }

        // CSend Tests

        [Test]
        public void CSend_ReceiverIdsIsNull_ShouldThrowException()
        {
            SendMessageData msg = u.GenerateChatSendMsgData();
            msg.ReceiverIds = null;

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            
            // make sure exception contains the phrase receiver id to indicate what's wrong
            Assert.That(Contains(e.Message, "receiver id"));
        }

        [Test]
        public void CSend_UnknownMessageType_ShouldThrowException()
        {
            SendMessageData msg = u.GenerateChatSendMsgData();
            msg.ReceiverIds = new int[0];

            // get an invalid enum value, take maximum + 1
            // this assumes that the value isn't maximum possible value of int
            msg.Type = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().Max();
            msg.Type = msg.Type + 1;

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            Assert.That(Contains(e.Message, "type"));
        }

        [Test]
        public void CSend_NullStringInChat_ShouldThrowException()
        {
            SendMessageData msg = u.GenerateChatSendMsgData();
            msg.Message = null;

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
        }

        [Test]
        public void CSend_NewMessageWithInvalidThreadId_ShouldThrowException()
        {
            SendMessageData msg = u.GenerateChatSendMsgData(replyId: (maxValidThreadId + 1));

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CSend(msg));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void CSend_ValidChatMessage_ShouldSendDataToServer()
        {
            SendMessageData msg = u.GenerateChatSendMsgData();
            msg.ReceiverIds = new int[0];

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            string sentData = fakeComm.GetSentData();
            MessageData sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(msg.Type, sentMsg.Type);
            Assert.AreEqual(msg.Message, sentMsg.Message);
            Assert.AreEqual(msg.ReceiverIds, sentMsg.ReceiverIds);
            Assert.IsNull(sentMsg.FileData);
            Assert.AreEqual(sentMsg.Event, MessageEvent.NewMessage);
            Assert.AreEqual(sentMsg.Starred, false);
            Assert.AreEqual(sentMsg.SenderId, cClient.UserId);
        }

        [Test]
        public void CSend_InvalidFilePath_ShouldThrowException()
        {
            SendMessageData msg = u.GenerateChatSendMsgData(type: MessageType.File);
            // give an invalid file path
            msg.Message = "";

            _ = Assert.Throws<FileNotFoundException>(() => cClient.CSend(msg));
        }

        [Test]
        public void CSend_ValidFileMessage_ShouldSendDataToServer()
        {
            SendMessageData msg = u.GenerateChatSendMsgData();
            msg.Type = MessageType.File;
            msg.ReceiverIds = new int[0];
            msg.Message = testfilepath;

            // send msg via CSend and capture sent data via fake communicator
            cClient.CSend(msg);
            string sentData = fakeComm.GetSentData();
            MessageData sentMsg = serializer.Deserialize<MessageData>(sentData);

            // read file data to be able to validate sent data
            SendFileData filedata = new SendFileData(testfilepath);

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
            int messageId = maxValidMsgId + 1;
            // savepath doesn't matter in this case
            string savepath = Path.GetRandomFileName();

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "not found"));
        }

        [Test]
        public void CDownload_AttemptToDownloadChatMessage_ShouldThrowException()
        {
            // give message id of chat message we simulated receiving
            int messageId = chatmsg.MessageId;
            // savepath shouldn't matter
            string savepath = Path.GetRandomFileName();

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "not a file"));
        }

        [Test]
        public void CDownload_InvalidPath_ShouldThrowException()
        {
            
            // give id of file we simulated receiving in SetUp method
            int messageId = filemsg.MessageId;
            // give a save path that isn't valid
            string savepath = "";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CDownload(messageId, savepath));
            Assert.That(Contains(e.Message, "path"));
        }

        [Test]
        public void CDownload_ValidDownload_ShouldSendDataToServer()
        {
            // use the correct message id
            int messageId = filemsg.MessageId;
            // a path that is writable (assuming directory with test file is writable)
            string savepath = Path.GetDirectoryName(testfilepath) + Path.GetRandomFileName();

            // request download and verify the data sent to the server (fake communicator) in this case
            cClient.CDownload(messageId, savepath);

            string sentData = fakeComm.GetSentData();
            MessageData sentMsg = serializer.Deserialize<MessageData>(sentData);

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
            int messageid = maxValidMsgId + 1;
            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CMarkStar(messageid));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void CMarkStar_StarringFileMessage_ShouldThrowException()
        {
            int messageid = filemsg.MessageId;
            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CMarkStar(messageid));
            Assert.That(Contains(e.Message, "not chat"));
        }

        [Test]
        public void CMarkStar_StarringMessage_ShouldSendDataToServer()
        {
            int messageid = chatmsg.MessageId;
            cClient.CMarkStar(messageid);

            // validate the captured data sent to the server
            string sentData = fakeComm.GetSentData();
            MessageData sentMsg = serializer.Deserialize<MessageData>(sentData);

            Assert.AreEqual(sentMsg.Event, MessageEvent.Star);
            Assert.AreEqual(sentMsg.MessageId, messageid);
        }

        // CUpdateChat tests
        [Test]
        public void CUpdateChat_InvalidMessageId_ShouldThrowException()
        {
            int messageid = maxValidMsgId + 1;
            string newMessage = "loremipsum";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void CUpdateChat_NonSenderUpdates_ShouldThrowException()
        {
            int messageid = chatmsg.MessageId;
            string newMessage = "loremipsum";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "sender"));
        }

        [Test]
        public void CUpdateChat_UpdatingFileMessge_ShouldThrowException()
        {
            int messageid = filemsg.MessageId;
            string newMessage = "loremipsum";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "not chat"));
        }

        [Test]
        public void CUpdateChat_NullMessage_ShouldThrowException()
        {
            int messageid = userchatmsg.MessageId;
            string newMessage = null;

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "invalid message"));
        }

        [Test]
        public void CUpdateChat_EmptyMessage_ShouldThrowException()
        {
            int messageid = userchatmsg.MessageId;
            string newMessage = "";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CUpdateChat(messageid, newMessage));
            Assert.That(Contains(e.Message, "invalid message"));
        }

        [Test]
        public void CUpdateChat_ValidUpdate_ShouldSendDataToServer()
        {
            int messageid = userchatmsg.MessageId;
            string newMessage = "loremipsum";

            cClient.CUpdateChat(messageid, newMessage);

            string sentData = fakeComm.GetSentData();
            MessageData sentMsg = serializer.Deserialize<MessageData>(sentData);

            // validate necessary fields
            Assert.AreEqual(sentMsg.Event, MessageEvent.Update);
            Assert.AreEqual(sentMsg.MessageId, messageid);
            Assert.AreEqual(sentMsg.Message, newMessage);
        }

        // CSubscribe tests

        [Test]
        public void CSubscribe_NullArgument_ShouldThrowException()
        {
            IContentListener subscriber = null;
            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CSubscribe(subscriber));
            Assert.That(Contains(e.Message, "null"));
        }

        // CGetThread tests

        [Test]
        public void CGetThread_InvalidThreadId_ShouldThrowException()
        {
            int threadId = maxValidThreadId + 1;
            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.CGetThread(threadId));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void CGetThread_ValidThreadId_ShouldReturnThread()
        {
            // choose the thread which contains chatmsg and filemsg
            int threadId = chatmsg.ReplyThreadId;
            ChatContext context = cClient.CGetThread(threadId);

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
            List<ChatContext> contexts = cClient.AllMessages;

            // modify contexts by deleting first element
            contexts.RemoveAt(0);
            
            // simulate the client receiving them
            cClient.OnReceive(contexts);

            // check if all messages field is set
            List<ChatContext> newContexts = cClient.AllMessages;

            Assert.AreEqual(contexts.Count, newContexts.Count);
            for (int i = 0; i < contexts.Count; i++)
            {
                AssertAreEqualDeep(contexts[i], newContexts[i]);
            }

            // check if subscribers are informed
            List<ChatContext> rcvdContexts = listener.GetOnAllMessagesData();

            Assert.AreEqual(contexts.Count, rcvdContexts.Count);
            for (int i = 0; i < contexts.Count; i++)
            {
                AssertAreEqualDeep(contexts[i], rcvdContexts[i]);
            }
        }

        // OnReceive(MessageData) tests

        [Test]
        public void OnReceive_NewMessage_InvalidThreadId_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = -1; // invalid thread id
            msg.Message = "Hello";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "thread id"));
        }

        [Test]
        public void OnReceive_NewMessage_DuplicateMessageIdExistingThread_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = chatmsg.MessageId; // reuse a message id
            msg.ReplyThreadId = maxValidThreadId; // reuse a thread id as well
            msg.Message = "Hello";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_NewMessage_DuplicateMessageIdNewThread_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = chatmsg.MessageId; // reuse a message id
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = "Hello";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_NewMessage_EmptyMessageString_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = "";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message string"));
        }

        [Test]
        public void OnReceive_NewMessage_NullMessageString_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId;
            msg.Message = null;

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message string"));
        }

        [Test]
        public void OnReceive_ValidNewMessage_NewThread_ShouldStoreMessageAndInformSubscribers()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.Chat;
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = ++maxValidThreadId; // new message thread
            msg.Message = "Hello";
            msg.SenderId = 401; // doesn't really matter

            cClient.OnReceive(msg);

            // validate that the message has been stored
            ChatContext msgContext = cClient.CGetThread(msg.ReplyThreadId);
            int index = msgContext.RetrieveMessageIndex(msg.MessageId);
            ReceiveMessageData storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            ReceiveMessageData recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_ValidNewMessage_ExistingThread_ShouldStoreMessageAndInformSubscribers()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.NewMessage;
            msg.Type = MessageType.File; // shouldn't matter if file or chat
            msg.MessageId = ++maxValidMsgId;
            msg.ReplyThreadId = chatmsg.ReplyThreadId; // existing message thread id
            msg.Message = "Hello";
            msg.SenderId = 401; // doesn't really matter

            cClient.OnReceive(msg);

            // validate that the message has been stored
            ChatContext msgContext = cClient.CGetThread(msg.ReplyThreadId);
            int index = msgContext.RetrieveMessageIndex(msg.MessageId);
            ReceiveMessageData storedMsg = msgContext.MsgList[index];
            AssertAreEqualDeep(msg, storedMsg);

            // validate that the subscribers have also received data
            ReceiveMessageData recvdMsg = listener.GetOnMessageData();
            AssertAreEqualDeep(msg, recvdMsg);
        }

        [Test]
        public void OnReceive_MessageUpdate_InvalidMessageId_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = ++maxValidMsgId; // try to update a message that doesn't exist
            msg.Message = "New message";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_MessageUpdate_FileMessage_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = filemsg.MessageId; // try to update a message that corresponds to a file
            msg.Message = "New message";

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
        }

        [Test]
        public void OnReceive_MessageUpdate_ValidUpdate_ShouldUpdateAndInformSubscribers()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = chatmsg.MessageId;
            msg.Message = "New message";

            cClient.OnReceive(msg);

            // validate that the message has been updated
            ChatContext msgContext = cClient.CGetThread(chatmsg.ReplyThreadId);
            int index = msgContext.RetrieveMessageIndex(chatmsg.MessageId);
            ReceiveMessageData storedMsg = msgContext.MsgList[index];
            Assert.AreEqual(storedMsg.Message, msg.Message);

            // validate that the subscribers have also received notification for update
            ReceiveMessageData recvdMsg = listener.GetOnMessageData();
            Assert.AreEqual(recvdMsg.Event, MessageEvent.Update);
            Assert.AreEqual(recvdMsg.MessageId, chatmsg.MessageId);
            Assert.AreEqual(recvdMsg.Message, msg.Message);
        }

        [Test]
        public void OnReceive_MessageStar_InvalidMessageId_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.MessageId = ++maxValidMsgId; // try to star a message that doesn't exist

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
            Assert.That(Contains(e.Message, "message id"));
        }

        [Test]
        public void OnReceive_MessageStar_FileMessage_ShouldThrowException()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Star;
            msg.MessageId = filemsg.MessageId; // try to star a message that corresponds to a file

            ArgumentException e = Assert.Throws<ArgumentException>(() => cClient.OnReceive(msg));
        }

        [Test]
        public void OnReceive_MessageStar_ValidUpdate_ShouldUpdateAndInformSubscribers()
        {
            MessageData msg = new MessageData();
            msg.Event = MessageEvent.Update;
            msg.MessageId = chatmsg.MessageId;
            msg.Message = "New message";

            cClient.OnReceive(msg);

            // validate that the message has been updated
            ChatContext msgContext = cClient.CGetThread(chatmsg.ReplyThreadId);
            int index = msgContext.RetrieveMessageIndex(chatmsg.MessageId);
            ReceiveMessageData storedMsg = msgContext.MsgList[index];
            Assert.AreEqual(storedMsg.Message, msg.Message);

            // validate that the subscribers have also received notification for update
            ReceiveMessageData recvdMsg = listener.GetOnMessageData();
            Assert.AreEqual(recvdMsg.Event, MessageEvent.Update);
            Assert.AreEqual(recvdMsg.MessageId, chatmsg.MessageId);
            Assert.AreEqual(recvdMsg.Message, msg.Message);
        }

        // helper functions to assert deep equality of data structures that the Content module uses

        public static void AssertAreEqualDeep(SendMessageData msg1, SendMessageData msg2)
        {
            Assert.AreEqual(msg1.Message, msg2.Message);
            Assert.AreEqual(msg1.ReceiverIds, msg2.ReceiverIds);
            Assert.AreEqual(msg1.Type, msg2.Type);
            Assert.AreEqual(msg1.ReplyThreadId, msg2.ReplyThreadId);
        }

        public static void AssertAreEqualDeep(ReceiveMessageData msg1, ReceiveMessageData msg2)
        {
            Assert.AreEqual(msg1.Event, msg2.Event);
            Assert.AreEqual(msg1.Type, msg2.Type);
            Assert.AreEqual(msg1.MessageId, msg2.MessageId);
            Assert.AreEqual(msg1.Message, msg2.Message);
            Assert.AreEqual(msg1.ReceiverIds, msg2.ReceiverIds);
            Assert.AreEqual(msg1.ReplyThreadId, msg2.ReplyThreadId);
            Assert.AreEqual(msg1.SenderId, msg2.SenderId);
            Assert.AreEqual(msg1.SentTime, msg2.SentTime);
            Assert.AreEqual(msg1.Starred, msg2.Starred);
        }

        public static void AssertAreEqualDeep(MessageData msg1, MessageData msg2)
        {
            AssertAreEqualDeep((ReceiveMessageData)msg1, (ReceiveMessageData)msg2);
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
            for (int i = 0; i < context1.NumOfMessages; i++)
            {
                AssertAreEqualDeep(context1.MsgList[i], context2.MsgList[i]);
            }
        }
    }
}