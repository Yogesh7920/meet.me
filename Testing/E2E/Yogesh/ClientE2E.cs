/// <author>Yogesh</author>
/// <created>26/11/2021</created>

using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Client.ViewModel;
using Content;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    [TestFixture]
    [NonParallelizable]
    [SingleThreaded]
    public class ClientE2E
    {
        private ISerializer _serializer;


        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _serializer = new Serializer();
        }

        [Test]
        [TestCase("127.0.0.1", 8080, "Yogesh", false)]
        [TestCase("127.0.0.1", 8080, "    ", true)] // username cannot be space.
        [TestCase("127.0.0.1", 8080, null, true)] // username cannot be null
        public void ClientArrival(string ip, int port, string username, bool error)
        {
            var authViewModel = new AuthViewModel();
            var valid = authViewModel.SendForAuth("127.0.0.1", 8080, username);
            Assert.AreEqual(valid, !error);
            if (error) return;
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(data.eventType, "addClient");
            Assert.AreEqual(data.username, username);
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
        }

        // [Test]
        // public void SendChatWithReply(string message, int replyId, bool error)
        // {
        //     var chatViewModel = new ChatViewModel();
        //     chatViewModel.SendChat("Hello", -1);
        //     var serializedData = File.ReadAllText("networking_output.json");
        //     // var content = ContentClientFactory.GetInstance();
        //     // var data = _serializer.Deserialize<MessageData>(serializedData);
        //
        // }

        [Test]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", -1, false)]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", -2, true)] // Reply ID -2 is invalid
        [TestCase("../../../DesignSpec.pdf", "DesignSpec.pdf", -2, true)] // File does not exist in this path
        public void SendFile(string filepath, string filename, int replyId, bool error)
        {
            var chatViewModel = new ChatViewModel();

            if (error)
            {
                Assert.That(() => chatViewModel.SendChat(filepath, replyId), Throws.Exception);
                return;
            }
            
            chatViewModel.SendFile(filepath, replyId);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, filename);
            Assert.AreEqual(data.ReplyMsgId, replyId);
        }

        [Test]
        public void SendWhiteboard()
        {
            
        }

        [Test]
        public void SendScreenShare()
        {
            
        }
    }
}