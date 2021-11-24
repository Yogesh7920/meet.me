using Client.ViewModel;
using Content;
using Dashboard;
using NUnit.Framework;
using System;
using static Testing.Client.Chat.ChatUtils;

namespace Testing.Client.Chat
{
    [TestFixture]
    public class ChatUnitTest
    {

        private ChatViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _viewModel = new ChatViewModel();
        }

        [Test]
        public void OnClientSessionChanged_ShouldAddUsers()
        {
            // Arrange
            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Suchitra", 111801043);
            UserData sampleUser2 = new UserData("Irene", 111801017);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            // Act
            _viewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel._users.Count, 2);
            Assert.AreEqual(_viewModel._users[111801043],"Suchitra");
            Assert.AreEqual(_viewModel._users[111801017],"Irene");
        }

        [Test]
        public void ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            ReceiveMessageData sampleMessageData = new ReceiveMessageData();
            sampleMessageData.Event = MessageEvent.NewMessage;
            sampleMessageData.Message = "Hi Suchitra, how are you???";
            sampleMessageData.MessageId = 1;
            sampleMessageData.ReceiverIds = new int[0];
            sampleMessageData.ReplyThreadId = -1;
            sampleMessageData.SenderId = 111801017;
            sampleMessageData.SentTime = DateTime.Now;
            sampleMessageData.Starred = false;
            sampleMessageData.Type = MessageType.Chat;
            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Suchitra", 111801043);
            UserData sampleUser2 = new UserData("Irene", 111801017);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);
            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnMessage(sampleMessageData);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.ReceivedMsg.UserName, "Irene");
        }

    }
}
