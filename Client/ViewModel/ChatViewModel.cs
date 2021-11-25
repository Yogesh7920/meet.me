/// <author>Suchitra Yechuri</author>
/// <created>12/11/2021</created>
/// <summary>
///     This file contains some mock objects which can
///     be used to simulate tests for the networking module.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Dashboard.Client.SessionManagement;
using Dashboard;
using Content;

namespace Client.ViewModel
{
    public class ChatViewModel :
        INotifyPropertyChanged, // Notifies clients that a property value has changed.
        IContentListener, // Notifies clients that has a message has been received.
        IClientSessionNotifications
    {

        public IDictionary<int, string> Messages;
        public IDictionary<int, string> Users;
        public static int UserId
        {
            get; private set;
        }
        /// <summary>
        /// The received caption.
        /// </summary>
        public Message ReceivedMsg
        {
            get; private set;
        }
        public ChatViewModel()
        {
            Messages = new Dictionary<int, string>();
            Users = new Dictionary<int, string>();
            _model = ContentClientFactory.GetInstance();
            _model.CSubscribe(this);
            UserId = _model.GetUserId();

            System.Diagnostics.Debug.WriteLine("userid: " + UserId);

            _modelDb = SessionManagerFactory.GetClientSessionManager();
            _modelDb.SubscribeSession(this);
        }

        public void SendChat(string message, int replyMsgId)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.Chat;
            msg.Message = message;
            msg.ReplyThreadId = replyMsgId;
            System.Diagnostics.Debug.WriteLine(msg.ReplyThreadId);
            msg.ReceiverIds = new int[] { };
            _model.CSend(msg);
        }

        public void SendFile(string message, int replyMsgId)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.File;
            msg.Message = message;
            msg.ReplyThreadId = replyMsgId;
            msg.ReceiverIds = new int[] { };
            _model.CSend(msg);
        }
        public void StarChat(int msgId)
        {
            _model.CMarkStar(msgId);
        }
        public void DownloadFile(int msgId, string path)
        {
            _model.CDownload(msgId, path);
        }
        public void OnMessage(ReceiveMessageData messageData)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<ReceiveMessageData>(messageData =>
                        {
                            lock (this)
                            {

                                if (messageData.Event == MessageEvent.NewMessage)
                                {
                                    Messages.Add(messageData.MessageId, messageData.Message);
                                    ReceivedMsg = new Message();
                                    ReceivedMsg.MessageId = messageData.MessageId;
                                    ReceivedMsg.UserName = Users[messageData.SenderId];
                                    ReceivedMsg.TextMessage = messageData.Message;
                                    ReceivedMsg.Time = messageData.SentTime.ToShortTimeString();
                                    ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                                    ReceivedMsg.ReplyMessage = messageData.ReplyThreadId == messageData.MessageId ? "" : Messages[messageData.ReplyThreadId];
                                    ReceivedMsg.Type = messageData.Type == MessageType.Chat;
                                    this.OnPropertyChanged("ReceivedMsg");
                                }
                            }
                        }),
                        messageData);
        }

        public void OnClientSessionChanged(SessionData session)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<SessionData>(session =>
                        {
                            lock (this)
                            {
                                Users.Clear();
                                foreach (UserData user in session.users)
                                {
                                    //System.Diagnostics.Debug.WriteLine(user.username);
                                    Users.Add(user.userID, user.username);
                                }
                            }
                        }),
                        session);
        }

        public void OnAllMessages(List<ChatContext> allMessages)
        {
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<List<ChatContext>>(allMessages =>
                        {
                            lock (this)
                            {
                                foreach (ChatContext msgLst in allMessages)
                                {
                                    foreach (ReceiveMessageData messageData in msgLst.MsgList)
                                    {
                                        Messages.Add(messageData.MessageId, messageData.Message);
                                        ReceivedMsg = new Message();
                                        ReceivedMsg.MessageId = messageData.MessageId;
                                        ReceivedMsg.UserName = Users[messageData.SenderId];
                                        ReceivedMsg.TextMessage = messageData.Message;
                                        ReceivedMsg.Time = messageData.SentTime.ToShortTimeString();
                                        ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                                        ReceivedMsg.ReplyMessage = messageData.ReplyThreadId == -1 ? "" : Messages[messageData.ReplyThreadId];
                                        ReceivedMsg.Type = messageData.Type == MessageType.Chat;
                                        this.OnPropertyChanged("ReceivedMsg");
                                    }
                                }
                            }
                        }),
                        allMessages);

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Gets the dispatcher to the main thread. In case it is not available
        /// (such as during unit testing) the dispatcher associated with the
        /// current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            (Application.Current?.Dispatcher != null) ?
                    Application.Current.Dispatcher :
                    Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Underlying data model.
        /// </summary>
        private IContentClient _model;
        private IUXClientSessionManager _modelDb;
    }
}
