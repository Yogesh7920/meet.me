/// <author>Suchitra Yechuri</author>
/// <created>2/11/2021</created>
/// <summary>
/// ViewModel for the Chat page.
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
        INotifyPropertyChanged,
        IContentListener,
        IClientSessionNotifications
    {
        /// <summary>
        /// Message ids along with corresponding message strings.
        /// </summary>
        public IDictionary<int, string> Messages;

        /// <summary>
        /// User ids along with user names.
        /// </summary>
        public IDictionary<int, string> Users;

        /// <summary>
        /// Message ids and their corresponding thread ids.
        /// </summary>
        public IDictionary<int, int> ThreadIds;

        /// <summary>
        /// Current user id.
        /// </summary>
        public static int UserId
        {
            get; private set;
        }

        /// <summary>
        /// Message to send.
        /// </summary>
        public SendMessageData MsgToSend
        {
            get; private set;
        }
        /// <summary>
        /// Received message.
        /// </summary>
        public Message ReceivedMsg
        {
            get; private set;
        }

        /// <summary>
        /// Creates an instance of the Chat ViewModel.
        /// </summary>
        public ChatViewModel()
        {
            Messages = new Dictionary<int, string>();
            Users = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();

            _model = ContentClientFactory.GetInstance();
            _model.CSubscribe(this);

            _modelDb = SessionManagerFactory.GetClientSessionManager();
            _modelDb.SubscribeSession(this);
        }

        /// <summary>
        /// Send chat message to the content module.
        /// </summary>
        /// <param name="message"> The message string </param>
        /// <param name="replyMsgId"> Reply id of the message replied to, -1 if not a reply </param>
        public void SendChat(string message, int replyMsgId)
        {
            MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.Chat;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };

            _model.CSend(MsgToSend);
        }

        /// <summary>
        /// Send file message to the content module.
        /// </summary>
        /// <param name="message"> The message string containing the file path </param>
        /// <param name="replyMsgId"> Reply id of the message replied to, -1 if not a reply </param>
        public void SendFile(string message, int replyMsgId)
        {
            MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.File;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };

            _model.CSend(MsgToSend);
        }

        /// <summary>
        /// Inform content module that a message is starred.
        /// </summary>
        /// <param name="msgId"> The message id of the message </param>
        public void StarChat(int msgId)
        {
            _model.CMarkStar(msgId);
        }

        /// <summary>
        /// Inform content module that a file needs to be downloaded.
        /// </summary>
        /// <param name="msgId"> The message id of the message </param>
        /// <param name="path"> The path to store the downloaded file in </param>
        public void DownloadFile(int msgId, string path)
        {
            _model.CDownload(msgId, path);
        }

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="messageData">The message object</param>
        public void OnMessage(ReceiveMessageData messageData)
        {
            // Execute the call on the application's main thread.
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
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
                                    UserId = _model.GetUserId();
                                    ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                                    ReceivedMsg.ReplyMessage = messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
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
                                if(session != null)
                                {
                                    Users.Clear();
                                    foreach (UserData user in session.users)
                                    {
                                        Users.Add(user.userID, user.username);
                                    }
                                }
                            }
                        }),
                        session);
        }

        /// <summary>
        /// Handles incoming list of messages when a new user joins.
        /// </summary>
        /// <param name="allMessages">The list of all messages</param>
        public void OnAllMessages(List<ChatContext> allMessages)
        {
            // Execute the call on the application's main thread.
            _ = this.ApplicationMainThreadDispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new Action<List<ChatContext>>(allMessages =>
                        {
                            lock (this)
                            {
                                Messages.Clear();
                                ThreadIds.Clear();
                                foreach (ChatContext msgLst in allMessages)
                                {
                                    foreach (ReceiveMessageData messageData in msgLst.MsgList)
                                    {
                                        Messages.Add(messageData.MessageId, messageData.Message);
                                        ThreadIds.Add(messageData.MessageId, messageData.ReplyThreadId);
                                        ReceivedMsg = new Message();
                                        ReceivedMsg.MessageId = messageData.MessageId;
                                        ReceivedMsg.UserName = Users[messageData.SenderId];
                                        ReceivedMsg.TextMessage = messageData.Message;
                                        ReceivedMsg.Time = messageData.SentTime.ToString("hh:mm tt");
                                        UserId = _model.GetUserId();
                                        ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                                        ReceivedMsg.ReplyMessage = messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
                                        ReceivedMsg.Type = messageData.Type == MessageType.Chat;
                                        this.OnPropertyChanged("ReceivedMsgs");
                                    }
                                }
                            }
                        }),
                        allMessages);
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
        /// Underlying data models.
        /// </summary>
        private IContentClient _model;
        private IUXClientSessionManager _modelDb;
    }
}