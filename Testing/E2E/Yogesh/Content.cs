using System;
using System.IO;
using System.Threading;
using Client.ViewModel;
using Content;
using Networking;
using NUnit.Framework;
using Testing.UX.Chat;

namespace Testing.E2E.Yogesh
{
    [TestFixture]
    public class Content
    {
        private ISerializer _serializer;
        private ContentServer _contentServer;
        private ContentClient _contentClient;
        
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _serializer = new Serializer();
            _contentServer = new ContentServer();
            _contentClient = new ContentClient();
        }
        
        [Test]
        [TestCase("Hi, I am Yogesh", -1, false)]
        [TestCase("Hi, I am Yogesh", 1, true)] // Reply ID 1 does not exist.
        [TestCase("Hello", -1, false)]
        [TestCase("Hi, I am Yogesh", -2, true)] // Reply ID -2 is invalid
        public void SendChat(string message, int replyId, bool error)
        {
            var chatViewModel = new ChatViewModel();

            if (error)
            {
                Assert.That(() => chatViewModel.SendChat(message, replyId), Throws.Exception);
                return;
            }
            
            chatViewModel.SendChat(message, replyId);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, message);
            Assert.AreEqual(data.ReplyMsgId, replyId);
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, message);
            Assert.AreEqual(data.ReplyMsgId, replyId);
            _contentClient.OnReceive(data);
        }
        
        [Test]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", -1, false)]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", -2, true)] // Reply ID -2 is invalid
        [TestCase("../../../DesignSpec.pdf", "DesignSpec.pdf", -2, true)] // File does not exist in this path
        public void SendFile(string filepath, string filename, int replyId, bool error)
        {
            var chatViewModel = new ChatViewModel();

            if (error)
            {
                Assert.That(() => chatViewModel.SendFile(filepath, replyId), Throws.Exception);
                return;
            }
            
            chatViewModel.SendFile(filepath, replyId);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, filename);
            Assert.AreEqual(data.ReplyMsgId, replyId);
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, filename);
            Assert.AreEqual(data.ReplyMsgId, replyId);
            _contentClient.OnReceive(data);
        }

        SendMessageData CreateChatReply(string message, int replyMsgId)
        {
            var MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.Chat;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = 0;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };
            return MsgToSend;
        }
        
        [Test]
        [TestCase("Hello World", 0)]
        public void SendChatWithReply(string message, int replyId)
        {
            var chatViewModel = new ChatViewModel();
            chatViewModel.SendChat("Hello world", -1);
            var serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);

            _contentClient.CSend(CreateChatReply(message, replyId));
            serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);
        }

        
        SendMessageData CreateFileReply(string message, int replyMsgId)
        {
            var MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.File;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = 0;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };
            return MsgToSend;
        }
        
        [Test]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", 0)]
        public void SendFileWithReply(string filepath, string filename, int replyId)
        {
            var chatViewModel = new ChatViewModel();
            chatViewModel.SendChat("Hello world", -1);
            var serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);
            
            _contentClient.CSend(CreateFileReply(filepath, 0));
            serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);
        }

        
        [Test]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf")]
        public void FetchFile(string filepath, string filename)
        {
            // var chatViewModel = new ChatViewModel();
            // chatViewModel.SendFile(filepath, -1);
            // var serializedData = File.ReadAllText("networking_output.json");
            // _contentServer.Receive(serializedData);
            // serializedData = File.ReadAllText("networking_output.json");
            // var data = _serializer.Deserialize<MessageData>(serializedData);
            // _contentClient.OnReceive(data);
            //
            // _contentClient.CDownload(0, "."); // TODO : Non-writable path.
            // serializedData = File.ReadAllText("networking_output.json");
            // _contentServer.Receive(serializedData);
            // serializedData = File.ReadAllText("networking_output.json");
            // data = _serializer.Deserialize<MessageData>(serializedData);
            // _contentClient.OnReceive(data);
        }

        [Test]
        public void StarMessage()
        {
            var chatViewModel = new ChatViewModel();
            chatViewModel.SendChat("Hello world", -1);
            var serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);
            
            _contentClient.CMarkStar(0);
            serializedData = File.ReadAllText("networking_output.json");
            _contentServer.Receive(serializedData);
            serializedData = File.ReadAllText("networking_output.json");
            data = _serializer.Deserialize<MessageData>(serializedData);
            _contentClient.OnReceive(data);
        }
    }
}