using Client;
using Client.ViewModel;
using Content;
using Dashboard;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Testing.UX.Chat.ChatUtils;

namespace Testing.UX.Chat
{
    [TestFixture]
    public class ChatUnitTests
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
            Assert.AreEqual(_viewModel.Users.Count, 2);
            Assert.AreEqual(_viewModel.Users[111801043],"Suchitra");
            Assert.AreEqual(_viewModel.Users[111801017],"Irene");
        }

        [Test]
        public void OnMessage_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            ReceiveMessageData sampleMessageData = new ReceiveMessageData();
            sampleMessageData.Event = MessageEvent.NewMessage;
            sampleMessageData.Message = "Hi Suchitra, how are you???";
            sampleMessageData.MessageId = 1;
            sampleMessageData.ReceiverIds = new int[0];
            sampleMessageData.ReplyMsgId = -1;
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
            Assert.AreEqual(_viewModel.ReceivedMsg.MessageId, 1);
            Assert.AreEqual(_viewModel.ReceivedMsg.TextMessage, "Hi Suchitra, how are you???");
            Assert.AreEqual(_viewModel.ReceivedMsg.ReplyMessage, "");
            Assert.AreEqual(_viewModel.ReceivedMsg.Type, true);

        }

        [Test]
        public void OnAllMessages_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            List<ChatContext> sampleAllMessages = new List<ChatContext>();
            ChatContext sampleChatContext = new ChatContext();
            ReceiveMessageData sampleMessageData1 = new ReceiveMessageData();
            ReceiveMessageData sampleMessageData2 = new ReceiveMessageData();

            sampleMessageData1.Event = MessageEvent.NewMessage;
            sampleMessageData1.Message = "Hi Suchitra, how are you???";
            sampleMessageData1.MessageId = 1;
            sampleMessageData1.ReceiverIds = new int[0];
            sampleMessageData1.ReplyMsgId = -1;
            sampleMessageData1.SenderId = 111801017;
            sampleMessageData1.SentTime = DateTime.Now;
            sampleMessageData1.Starred = false;
            sampleMessageData1.Type = MessageType.Chat;

            sampleMessageData2.Event = MessageEvent.NewMessage;
            sampleMessageData2.Message = "I'm fine, what about you?";
            sampleMessageData2.MessageId = 2;
            sampleMessageData2.ReceiverIds = new int[0];
            sampleMessageData2.ReplyMsgId = 1;
            sampleMessageData2.SenderId = 111801043;
            sampleMessageData2.SentTime = DateTime.Now;
            sampleMessageData2.Starred = false;
            sampleMessageData2.Type = MessageType.Chat;

            List<ReceiveMessageData> sampleMsgList = new List<ReceiveMessageData>();
            sampleMsgList.Add(sampleMessageData1);
            sampleMsgList.Add(sampleMessageData2);
            sampleChatContext.MsgList = sampleMsgList;
            sampleAllMessages.Add(sampleChatContext);

            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Suchitra", 111801043);
            UserData sampleUser2 = new UserData("Irene", 111801017);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);
            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnAllMessages(sampleAllMessages);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.ReceivedMsg.UserName, "Suchitra");
            Assert.AreEqual(_viewModel.ReceivedMsg.MessageId, 2);
            Assert.AreEqual(_viewModel.ReceivedMsg.TextMessage, "I'm fine, what about you?");
            Assert.AreEqual(_viewModel.ReceivedMsg.ReplyMessage, "Hi Suchitra, how are you???");
            Assert.AreEqual(_viewModel.ReceivedMsg.Type, true);
        }

        [Test]
        public void OnPropertyChanged_EventShouldBeRaised()
        {
            //Arrange
            string samplePropertyName = "";
            _viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                samplePropertyName = e.PropertyName;
            };

            //Act
            _viewModel.OnPropertyChanged("testing");

            //Assert
            Assert.IsNotNull(samplePropertyName);
            Assert.AreEqual("testing", samplePropertyName);
        }

        [Test]
        public void SendChatMessage_ShouldMatchMsgToBeSent()
        {
            //Arrange
            string sampleMessage = "Hello World!!";
            int sampleReplyMsgId = -1;

            //Act
            _viewModel.SendChat(sampleMessage, sampleReplyMsgId);

            //Assert
            Assert.IsTrue(_viewModel.MsgToSend.ReplyThreadId == -1);

        }

        [Test]
        public void SendFileMessage_ShouldMatchMsgToBeSent()
        {
            //Arrange
            string sampleMessage = "C:\\Users\\suchi\\Downloads\\Test_File.pdf";
            int sampleReplyMsgId = -1;

            //Act
            _viewModel.SendFile(sampleMessage, sampleReplyMsgId);

            //Assert
            Assert.IsTrue(_viewModel.MsgToSend.ReplyThreadId == -1);

        }

    }
}
