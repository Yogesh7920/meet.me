/// <author>Suchitra Yechuri</author>
/// <created>23/11/2021</created>
/// <summary>
///     This file contains all the chat unit tests.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Client.ViewModel;
using Content;
using Dashboard;
using NUnit.Framework;
using static Testing.UX.Chat.ChatUtils;

namespace Testing.UX.Chat
{
    [TestFixture]
    public class ChatUnitTests
    {
        [SetUp]
        public void SetUp()
        {
            _viewModel = new ChatViewModel(true);
        }

        private ChatViewModel _viewModel;

        /// <summary>
        ///     Checking whether the OnClientSessionChanged function updates the user list
        /// </summary>
        [Test]
        public void OnClientSessionChanged_ShouldAddUsers()
        {
            // Arrange
            var sampleSession = new SessionData();
            var sampleUser1 = new UserData("Suchitra", 111801043);
            var sampleUser2 = new UserData("Irene", 111801017);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            // Act
            _viewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.Users.Count, 2);
            Assert.AreEqual(_viewModel.Users[111801043], "Suchitra");
            Assert.AreEqual(_viewModel.Users[111801017], "Irene");
        }

        /// <summary>
        ///     Checking whether the message data received from the content module
        ///     through the OnMessage function matchs the ReceivedMsg object
        /// </summary>
        [Test]
        public void OnMessage_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            var sampleMessageData = new ReceiveMessageData();
            sampleMessageData.Event = MessageEvent.NewMessage;
            sampleMessageData.Message = "Hi Suchitra, how are you???";
            sampleMessageData.MessageId = 1;
            sampleMessageData.ReceiverIds = new int[0];
            sampleMessageData.ReplyMsgId = -1;
            sampleMessageData.SenderId = 111801017;
            sampleMessageData.SentTime = DateTime.Now;
            sampleMessageData.Starred = false;
            sampleMessageData.Type = MessageType.Chat;
            var sampleSession = new SessionData();
            var sampleUser1 = new UserData("Suchitra", 111801043);
            var sampleUser2 = new UserData("Irene", 111801017);
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

        /// <summary>
        ///     Checking whether the messages data received from the content module
        ///     through the OnAllMessages function matchs the ReceivedMsg object
        /// </summary>
        [Test]
        public void OnAllMessages_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            var sampleAllMessages = new List<ChatContext>();
            var sampleChatContext = new ChatContext();
            var sampleMessageData1 = new ReceiveMessageData();
            var sampleMessageData2 = new ReceiveMessageData();

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

            var sampleMsgList = new List<ReceiveMessageData>();
            sampleMsgList.Add(sampleMessageData1);
            sampleMsgList.Add(sampleMessageData2);
            sampleChatContext.MsgList = sampleMsgList;
            sampleAllMessages.Add(sampleChatContext);

            var sampleSession = new SessionData();
            var sampleUser1 = new UserData("Suchitra", 111801043);
            var sampleUser2 = new UserData("Irene", 111801017);
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

        /// <summary>
        ///     Checking whether event is raised on OnPropertyChanged
        /// </summary>
        [Test]
        public void OnPropertyChanged_EventShouldBeRaised()
        {
            //Arrange
            var samplePropertyName = "";
            _viewModel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                samplePropertyName = e.PropertyName;
            };

            //Act
            _viewModel.OnPropertyChanged("testing");

            //Assert
            Assert.IsNotNull(samplePropertyName);
            Assert.AreEqual("testing", samplePropertyName);
        }

        /// <summary>
        ///     Checking whether the chat message to be sent matchs the MsgToSend object
        /// </summary>
        [Test]
        public void SendChatMessage_ShouldMatchMsgToBeSent()
        {
            //Arrange
            var sampleMessage = "Hello World!!";
            var sampleReplyMsgId = -1;

            //Act
            _viewModel.SendChat(sampleMessage, sampleReplyMsgId);

            //Assert
            Assert.IsTrue(_viewModel.MsgToSend.ReplyThreadId == -1);
        }

        /// <summary>
        ///     Checking whether the file message to be sent matchs the MsgToSend object
        /// </summary>
        [Test]
        public void SendFileMessage_ShouldMatchMsgToBeSent()
        {
            //Arrange
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = currentDirectory.Split(new[] {"\\Testing"}, StringSplitOptions.None);
            var sampleMessage = path[0] + "\\Testing\\UX\\Chat\\Test_File.pdf";
            var sampleReplyMsgId = -1;

            //Act
            _viewModel.SendFile(sampleMessage, sampleReplyMsgId);

            //Assert
            Assert.IsTrue(_viewModel.MsgToSend.ReplyThreadId == -1);
        }
    }
}