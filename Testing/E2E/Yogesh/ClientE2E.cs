/// <author>Yogesh</author>
/// <created>26/11/2021</created>

using System;
using System.IO;
using System.Threading;
using Client.ViewModel;
using Content;
using Dashboard;
using Dashboard.Server.SessionManagement;
using Networking;
using NUnit.Framework;

namespace Testing.E2E.Yogesh
{
    public class ClientE2E
    {
        private ISerializer _serializer;

        [OneTimeSetUp]
        public void Setup()
        {
            _serializer = new Serializer();
            Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
        }

        [Test]
        [TestCase("127.0.0.1", 8080, "Yogesh", false)]
        public void ClientArrival(string ip, int port, string username, bool error)
        {
            var authViewModel = new AuthViewModel();
            
            authViewModel.SendForAuth("127.0.0.1", 8080, username);
            
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<ClientToServerData>(serializedData);
            Assert.AreEqual(data.eventType, "addClient");
            Assert.AreEqual(data.username, username);
        }

        [Test]
        [TestCase("Hi, I am Yogesh", -1)]
        public void SendChat(string message, int replyId)
        {
            var chatViewModel = new ChatViewModel();
            chatViewModel.SendChat(message, replyId);
            var serializedData = File.ReadAllText("networking_output.json");
            var data = _serializer.Deserialize<MessageData>(serializedData);
            Assert.AreEqual(data.Message, message);
            Assert.AreEqual(data.ReplyMsgId, replyId);
        }

        [Test]
        [TestCase("../../../../DesignSpec.pdf", "DesignSpec.pdf", -1)]
        public void SendFile(string filepath, string filename, int replyId)
        {
            var chatViewModel = new ChatViewModel();
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