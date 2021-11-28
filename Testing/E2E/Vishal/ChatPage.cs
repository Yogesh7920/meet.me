/// <author>Vishal Rao</author>
/// <created>27/11/2021</created>
/// <summary>
///     This file contains testcases for ChatPage.
/// </summary>

using System.Collections.Generic;
using System;
using System.Threading;
using System.Net.Sockets;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using Networking;
using Dashboard;
using Content;
using Whiteboard;
using ScreenSharing;
using Client;
using Client.ViewModel;
using static Testing.E2E.Vishal.Utils;

namespace Testing.E2E.Vishal
{
    [TestFixture]
    public class E2ETestChatPage
    {
        private ChatViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            //Environment.SetEnvironmentVariable("TEST_MODE", "E2E");
            _viewModel = new ChatViewModel();
        }

        [OneTimeTearDown]
        public void Close()
        {
            //Environment.SetEnvironmentVariable("TEST_MODE", "false");
        }

        [Test]
        public void ClientJoin()
        {
            // Arrange
            SessionData sampleSession = new SessionData();
            // create users
            UserData sampleUser1 = new UserData("Vishal", 46);
            UserData sampleUser2 = new UserData("User2", 56);
            sampleSession.AddUser(sampleUser1);

            // Act
            _viewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.Users.Count, 1);

            sampleSession.AddUser(sampleUser2);
            // Act
            _viewModel.OnClientSessionChanged(sampleSession);

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.Users.Count, 2);
            Assert.AreEqual(_viewModel.Users[46], "Vishal");
            Assert.AreEqual(_viewModel.Users[56], "User2");
        }

        [Test]
        public void SendingAndReceivingChat()
        {
            //Arrange
            ReceiveMessageData sampleMessageData = new ReceiveMessageData();
            sampleMessageData.Event = MessageEvent.NewMessage;
            sampleMessageData.Message = "Hey Vishal";
            sampleMessageData.MessageId = 1;
            sampleMessageData.ReceiverIds = new int[0];
            sampleMessageData.ReplyMsgId = -1;
            sampleMessageData.SenderId = 56;
            sampleMessageData.SentTime = DateTime.Now;
            sampleMessageData.Starred = false;
            sampleMessageData.Type = MessageType.Chat;

            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Vishal", 46);
            UserData sampleUser2 = new UserData("User2", 56);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnMessage(sampleMessageData);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.ReceivedMsg.UserName, "User2");
            Assert.AreEqual(_viewModel.ReceivedMsg.MessageId, 1);
            Assert.AreEqual(_viewModel.ReceivedMsg.TextMessage, "Hey Vishal");
            Assert.AreEqual(_viewModel.ReceivedMsg.ReplyMessage, "");
            Assert.AreEqual(_viewModel.ReceivedMsg.Type, true);

        }

        [Test]
        public void ChatAndReply()
        {
            //Arrange
            List<ChatContext> sampleAllMessages = new List<ChatContext>();
            ChatContext sampleChatContext = new ChatContext();
            ReceiveMessageData sampleMessageData1 = new ReceiveMessageData();
            ReceiveMessageData sampleMessageData2 = new ReceiveMessageData();

            sampleMessageData1.Event = MessageEvent.NewMessage;
            sampleMessageData1.Message = "Hey Vishal";
            sampleMessageData1.MessageId = 1;
            sampleMessageData1.ReceiverIds = new int[0];
            sampleMessageData1.ReplyMsgId = -1;
            sampleMessageData1.SenderId = 56;
            sampleMessageData1.SentTime = DateTime.Now;
            sampleMessageData1.Starred = false;
            sampleMessageData1.Type = MessageType.Chat;

            sampleMessageData2.Event = MessageEvent.NewMessage;
            sampleMessageData2.Message = "Yes there";
            sampleMessageData2.MessageId = 2;
            sampleMessageData2.ReceiverIds = new int[0];
            sampleMessageData2.ReplyMsgId = 1;
            sampleMessageData2.SenderId = 46;
            sampleMessageData2.SentTime = DateTime.Now;
            sampleMessageData2.Starred = false;
            sampleMessageData2.Type = MessageType.Chat;

            List<ReceiveMessageData> sampleMsgList = new List<ReceiveMessageData>();
            sampleMsgList.Add(sampleMessageData1);
            sampleMsgList.Add(sampleMessageData2);
            sampleChatContext.MsgList = sampleMsgList;
            sampleAllMessages.Add(sampleChatContext);

            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Vishal", 46);
            UserData sampleUser2 = new UserData("User2", 56);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnAllMessages(sampleAllMessages);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.ReceivedMsg.UserName, "Vishal");
            Assert.AreEqual(_viewModel.ReceivedMsg.MessageId, 2);
            Assert.AreEqual(_viewModel.ReceivedMsg.TextMessage, "Yes there");
            Assert.AreEqual(_viewModel.ReceivedMsg.ReplyMessage, "Hey Vishal");
            Assert.AreEqual(_viewModel.ReceivedMsg.Type, true);
        }

        [Test]
        public void SendingAndReceivingFile()
        {
            //Arrange
            ReceiveMessageData sampleMessageData = new ReceiveMessageData();
            sampleMessageData.Event = MessageEvent.NewMessage;
            sampleMessageData.Message = "Foo.pdf";
            sampleMessageData.MessageId = 1;
            sampleMessageData.ReceiverIds = new int[0];
            sampleMessageData.ReplyMsgId = -1;
            sampleMessageData.SenderId = 56;
            sampleMessageData.SentTime = DateTime.Now;
            sampleMessageData.Starred = false;
            sampleMessageData.Type = MessageType.File;

            SessionData sampleSession = new SessionData();
            UserData sampleUser1 = new UserData("Vishal", 46);
            UserData sampleUser2 = new UserData("User2", 56);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);

            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnMessage(sampleMessageData);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            DispatcherUtil.DoEvents();
            Assert.AreEqual(_viewModel.ReceivedMsg.UserName, "User2");
            Assert.AreEqual(_viewModel.ReceivedMsg.MessageId, 1);
            Assert.AreEqual(_viewModel.ReceivedMsg.TextMessage, "Foo.pdf");
            Assert.AreEqual(_viewModel.ReceivedMsg.ReplyMessage, "");
            Assert.AreEqual(_viewModel.ReceivedMsg.Type, false);
        }
    }
}
