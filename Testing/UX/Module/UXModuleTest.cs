using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Client;
using Client.ViewModel;
using Content;
using Dashboard;
using Dashboard.Client.SessionManagement;
using NUnit.Framework;
using Testing.UX.Chat;

namespace Testing.UX.Module
{
    public class FakeUxClientSessionManager : IUXClientSessionManager
    {
        public string ipAddress;
        public int port;
        public string username;
        public event NotifySummaryCreated SummaryCreated;
        public event NotifyEndMeet MeetingEnded;
        public event NotifyAnalyticsCreated AnalyticsCreated;

        public bool AddClient(string ipAddress, int ports, string username)
        {
            this.ipAddress = ipAddress;
            port = ports;
            this.username = username;
            return true;
        }

        public void RemoveClient()
        {
            throw new NotImplementedException();
        }

        public void EndMeet()
        {
            throw new NotImplementedException();
        }

        public void GetSummary()
        {
            throw new NotImplementedException();
        }

        public void SubscribeSession(IClientSessionNotifications listener)
        {
            throw new NotImplementedException();
        }

        public void GetAnalytics()
        {
            throw new NotImplementedException();
        }

        public UserData GetUser()
        {
            throw new NotImplementedException();
        }
    }

    public class FakeChatViewModel : IClientSessionNotifications, IContentListener
    {
        public IDictionary<int, string> Messages;
        public IDictionary<int, int> ThreadIds;

        public IDictionary<int, string> Users;

        public Message ReceivedMsg { get; private set; }

        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        public void OnClientSessionChanged(SessionData session)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<SessionData>(session =>
                {
                    lock (this)
                    {
                        if (session != null)
                        {
                            Trace.WriteLine("[UX] Received users list");
                            Users.Clear();
                            foreach (var user in session.users) Users.Add(user.userID, user.username);
                        }
                    }
                }),
                session);
        }

        public void OnMessage(ReceiveMessageData messageData)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<ReceiveMessageData>(messageData =>
                {
                    lock (this)
                    {
                        if (messageData.Event == MessageEvent.NewMessage)
                        {
                            Messages.Add(messageData.MessageId, messageData.Message);
                            ThreadIds.Add(messageData.MessageId, messageData.ReplyThreadId);
                            ReceivedMsg = new Message();
                            ReceivedMsg.MessageId = messageData.MessageId;
                            ReceivedMsg.UserName = Users[messageData.SenderId];
                            ReceivedMsg.TextMessage = messageData.Message;
                            ReceivedMsg.Time = messageData.SentTime.ToString("hh:mm tt");
                            ReceivedMsg.ToFrom = true;
                            ReceivedMsg.ReplyMessage =
                                messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
                            ReceivedMsg.Type = messageData.Type == MessageType.Chat;
                        }
                    }
                }),
                messageData);
        }

        public void OnAllMessages(List<ChatContext> allMessages)
        {
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<List<ChatContext>>(allMessages =>
                {
                    lock (this)
                    {
                        Messages.Clear();
                        ThreadIds.Clear();
                        foreach (var msgLst in allMessages)
                        foreach (var messageData in msgLst.MsgList)
                        {
                            Trace.WriteLine("[UX] Received all messages");
                            Messages.Add(messageData.MessageId, messageData.Message);
                            ThreadIds.Add(messageData.MessageId, messageData.ReplyThreadId);

                            // Create the ReceivedMsg object and update the fields accordingly
                            ReceivedMsg = new Message();
                            ReceivedMsg.MessageId = messageData.MessageId;
                            ReceivedMsg.UserName = Users[messageData.SenderId];
                            ReceivedMsg.TextMessage = messageData.Message;
                            ReceivedMsg.Time = messageData.SentTime.ToString("hh:mm tt");
                            ReceivedMsg.ToFrom = true;
                            ReceivedMsg.ReplyMessage =
                                messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
                            ReceivedMsg.Type = messageData.Type == MessageType.Chat;
                        }
                    }
                }),
                allMessages);
        }
    }

    [TestFixture]
    public class UxClientModuleTesting
    {
        [OneTimeSetUp]
        public void Setup()
        {
            _authViewModel = new AuthViewModel(sm);
            _chatViewModel = new ChatViewModel(true);
        }

        private AuthViewModel _authViewModel;
        private ChatViewModel _chatViewModel;

        private readonly FakeUxClientSessionManager sm = new();
        private FakeChatViewModel cm = new();

        [Test]
        public void OnLogin_ShouldMatchCredentials()
        {
            var ip = "192.168.1.1";
            var port = 8080;
            var username = "Adam";
            _authViewModel.SendForAuth(ip, port, username);
            Assert.AreEqual(ip, sm.ipAddress);
            Assert.AreEqual(port, sm.port);
            Assert.AreEqual(username, sm.username);
        }

        [Test]
        public void OnClientSessionChanged_ShouldAddUsers()
        {
            // Arrange
            var testSession = new SessionData();
            var testUser1 = new UserData("David", 1);
            var testUser2 = new UserData("Goliath", 2);
            testSession.AddUser(testUser1);
            testSession.AddUser(testUser2);

            // Act
            _chatViewModel.OnClientSessionChanged(testSession);
            ChatUtils.DispatcherUtil.DoEvents();

            // Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail

            Assert.AreEqual(_chatViewModel.Users.Count, 2);
            Assert.AreEqual(_chatViewModel.Users[1], "David");
            Assert.AreEqual(_chatViewModel.Users[2], "Goliath");
        }

        [Test]
        public void OnAllMessages_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            var sampleAllMessages = new List<ChatContext>();
            var sampleChatContext = new ChatContext();
            var sampleMessageData1 = new ReceiveMessageData();
            var sampleMessageData2 = new ReceiveMessageData();

            sampleMessageData1.Event = MessageEvent.NewMessage;
            sampleMessageData1.Message = "Hi how are you???";
            sampleMessageData1.MessageId = 1;
            sampleMessageData1.ReceiverIds = new int[0];
            sampleMessageData1.ReplyMsgId = -1;
            sampleMessageData1.SenderId = 1;
            sampleMessageData1.SentTime = DateTime.Now;
            sampleMessageData1.Starred = false;
            sampleMessageData1.Type = MessageType.Chat;

            sampleMessageData2.Event = MessageEvent.NewMessage;
            sampleMessageData2.Message = "I'm fine, what about you?";
            sampleMessageData2.MessageId = 2;
            sampleMessageData2.ReceiverIds = new int[0];
            sampleMessageData2.ReplyMsgId = 1;
            sampleMessageData2.SenderId = 2;
            sampleMessageData2.SentTime = DateTime.Now;
            sampleMessageData2.Starred = false;
            sampleMessageData2.Type = MessageType.Chat;

            var sampleMsgList = new List<ReceiveMessageData>();
            sampleMsgList.Add(sampleMessageData1);
            sampleMsgList.Add(sampleMessageData2);
            sampleChatContext.MsgList = sampleMsgList;
            sampleAllMessages.Add(sampleChatContext);

            var sampleSession = new SessionData();
            var sampleUser1 = new UserData("David", 1);
            var sampleUser2 = new UserData("Goliath", 2);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);
            _chatViewModel.OnClientSessionChanged(sampleSession);
            ModuleUtils.DispatcherUtil.DoEvents();

            //Act
            _chatViewModel.OnAllMessages(sampleAllMessages);

            //Assert
            // Without calling DispatcherUtil.DoEvents() the test will fail
            ModuleUtils.DispatcherUtil.DoEvents();
            Assert.AreEqual(_chatViewModel.ReceivedMsg.UserName, "Goliath");
            Assert.AreEqual(_chatViewModel.ReceivedMsg.MessageId, 2);
            Assert.AreEqual(_chatViewModel.ReceivedMsg.TextMessage, "I'm fine, what about you?");
            Assert.AreEqual(_chatViewModel.ReceivedMsg.ReplyMessage, "Hi how are you???");
            Assert.AreEqual(_chatViewModel.ReceivedMsg.Type, true);
        }
    }
}