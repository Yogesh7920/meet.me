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

        IDictionary<int, string> _messages;
<<<<<<< Updated upstream
        private IDictionary<int, string> _users;
        public int UserId
=======
        public static int UserId
>>>>>>> Stashed changes
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
            _messages = new Dictionary<int, string>();
            _users = new Dictionary<int, string>();
            _model = ContentClientFactory.getInstance();
            _model.CSubscribe(this);
            this.UserId = _model.GetUserId();

            _modelDb = SessionManagerFactory.GetClientSessionManager();
            _modelDb.SubscribeSession(this);
        }

        public void SendChat(string message, int replyMsgId)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.Chat;
            msg.Message = message;
            msg.ReplyThreadId = replyMsgId;
            _model.CSend(msg);
        }

        public void SendFile(string message, int replyMsgId)
        {
            SendMessageData msg = new SendMessageData();
            msg.Type = MessageType.File;
            msg.Message = message;
            msg.ReplyThreadId = replyMsgId;
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

                                if(messageData.Event == MessageEvent.NewMessage)
                                {
                                    _messages.Add(messageData.MessageId, messageData.Message);
                                    ReceivedMsg = new Message();
                                    ReceivedMsg.MessageId = messageData.MessageId;
                                    ReceivedMsg.UserName = _users[messageData.MessageId];
                                    ReceivedMsg.TextMessage = messageData.Message;
                                    ReceivedMsg.Time = messageData.SentTime.ToString();
                                    ReceivedMsg.ToFrom = UserId == messageData.MessageId;
                                    ReceivedMsg.ReplyMessage = messageData.ReplyThreadId == -1 ? "" : _messages[messageData.ReplyThreadId];
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
                                _users.Clear();
                                foreach (UserData user in session.users)
                                {
                                    _users.Add(user.userID, user.username);
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
                                    foreach(ReceiveMessageData messageData in msgLst.MsgList)
                                    {
                                        _messages.Add(messageData.MessageId, messageData.Message);
                                        ReceivedMsg = new Message();
                                        ReceivedMsg.MessageId = messageData.MessageId;
                                        ReceivedMsg.UserName = _users[messageData.MessageId];
                                        ReceivedMsg.TextMessage = messageData.Message;
                                        ReceivedMsg.Time = messageData.SentTime.ToString();
                                        ReceivedMsg.ToFrom = UserId == messageData.MessageId;
                                        ReceivedMsg.ReplyMessage = messageData.ReplyThreadId == -1 ? "" : _messages[messageData.ReplyThreadId];
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
        private void OnPropertyChanged(string property)
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
