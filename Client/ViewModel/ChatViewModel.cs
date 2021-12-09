/// <author>Suchitra Yechuri</author>
/// <created>2/11/2021</created>
/// <summary>
///     ViewModel for the Chat page.
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Content;
using Dashboard;
using Dashboard.Client.SessionManagement;

namespace Client.ViewModel
{
    public class ChatViewModel :
        INotifyPropertyChanged,
        IContentListener,
        IClientSessionNotifications
    {
        /// <summary>
        ///     Underlying data models.
        /// </summary>
        private readonly IContentClient _model;

        private readonly IUXClientSessionManager _modelDb;

        /// <summary>
        ///     Message ids along with corresponding message strings.
        /// </summary>
        public IDictionary<int, string> Messages;

        /// <summary>
        ///     Message ids and their corresponding thread ids.
        /// </summary>
        public IDictionary<int, int> ThreadIds;

        /// <summary>
        ///     User ids along with user names.
        /// </summary>
        public IDictionary<int, string> Users;

        /// <summary>
        ///     Creates an instance of the Chat ViewModel.
        /// </summary>
        /// <param name="testing"> For testing pursose set as true </param>
        public ChatViewModel(bool testing = false)
        {
            Messages = new Dictionary<int, string>();
            Users = new Dictionary<int, string>();
            ThreadIds = new Dictionary<int, int>();

            Testing = testing;
            if (!testing)
            {
                _model = ContentClientFactory.GetInstance();
                // Subscribe to the content module for messages
                _model.CSubscribe(this);

                _modelDb = SessionManagerFactory.GetClientSessionManager();
                // Subscribe to the dashboard module for users list
                _modelDb.SubscribeSession(this);
            }
        }

        /// <summary>
        ///     Current user id.
        /// </summary>
        public static int UserId { get; private set; }

        /// <summary>
        ///     Message to send.
        /// </summary>
        public SendMessageData MsgToSend { get; private set; }

        /// <summary>
        ///     Received message.
        /// </summary>
        public Message ReceivedMsg { get; private set; }

        /// <summary>
        ///     Set true for testing purposes.
        /// </summary>
        public bool Testing { get; }

        /// <summary>
        ///     Gets the dispatcher to the main thread. In case it is not available
        ///     (such as during unit testing) the dispatcher associated with the
        ///     current thread is returned.
        /// </summary>
        private Dispatcher ApplicationMainThreadDispatcher =>
            Application.Current?.Dispatcher != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     Updates the users list with the incoming data.
        /// </summary>
        /// <param name="session"> Contains the list of all users </param>
        public void OnClientSessionChanged(SessionData session)
        {
            // Execute the call on the application's main thread.
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

        /// <summary>
        ///     Handles an incoming message.
        /// </summary>
        /// <param name="messageData">The message object</param>
        public void OnMessage(ReceiveMessageData messageData)
        {
            // Execute the call on the application's main thread.
            _ = ApplicationMainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<ReceiveMessageData>(messageData =>
                {
                    lock (this)
                    {
                        if (messageData.Event == MessageEvent.NewMessage)
                        {
                            Trace.WriteLine("[UX] Received new message");
                            Messages.Add(messageData.MessageId, messageData.Message);
                            ThreadIds.Add(messageData.MessageId, messageData.ReplyThreadId);

                            // Get the userid from the content module
                            if (!Testing) UserId = _model.GetUserId();

                            // Create the ReceivedMsg object and update the fields accordingly
                            ReceivedMsg = new Message();
                            ReceivedMsg.MessageId = messageData.MessageId;
                            ReceivedMsg.UserName = Users.ContainsKey(messageData.SenderId) ? Users[messageData.SenderId] : "Anonymous";
                            ReceivedMsg.TextMessage = messageData.Message;
                            ReceivedMsg.Time = messageData.SentTime.ToString("hh:mm tt");
                            ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                            ReceivedMsg.ReplyMessage =
                                messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
                            ReceivedMsg.Type = messageData.Type == MessageType.Chat;

                            OnPropertyChanged("ReceivedMsg");
                        }
                    }
                }),
                messageData);
        }

        /// <summary>
        ///     Handles incoming list of messages when a new user joins.
        /// </summary>
        /// <param name="allMessages">The list of all messages</param>
        public void OnAllMessages(List<ChatContext> allMessages)
        {
            // Execute the call on the application's main thread.
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

                            // Get the userid from the content module
                            if (!Testing) UserId = _model.GetUserId();

                            // Create the ReceivedMsg object and update the fields accordingly
                            ReceivedMsg = new Message();
                            ReceivedMsg.MessageId = messageData.MessageId;
                            ReceivedMsg.UserName = Users.ContainsKey(messageData.SenderId) ? Users[messageData.SenderId] : "Anonymous";
                            ReceivedMsg.TextMessage = messageData.Message;
                            ReceivedMsg.Time = messageData.SentTime.ToString("hh:mm tt");
                            ReceivedMsg.ToFrom = UserId == messageData.SenderId;
                            ReceivedMsg.ReplyMessage =
                                messageData.ReplyMsgId == -1 ? "" : Messages[messageData.ReplyMsgId];
                            ReceivedMsg.Type = messageData.Type == MessageType.Chat;

                            OnPropertyChanged("ReceivedMsgs");
                        }
                    }
                }),
                allMessages);
        }

        /// <summary>
        ///     Property changed event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Send chat message to the content module.
        /// </summary>
        /// <param name="message"> The message string </param>
        /// <param name="replyMsgId"> Reply id of the message replied to, -1 if not a reply </param>
        public void SendChat(string message, int replyMsgId)
        {
            // Create the SendMessageData object and update the fields accordingly
            MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.Chat;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };

            if (!Testing)
            {
                Trace.WriteLine("[UX] Sending chat message");
                _model.CSend(MsgToSend);
            }
        }

        /// <summary>
        ///     Send file message to the content module.
        /// </summary>
        /// <param name="message"> The message string containing the file path </param>
        /// <param name="replyMsgId"> Reply id of the message replied to, -1 if not a reply </param>
        public void SendFile(string message, int replyMsgId)
        {
            // Create the SendMessageData object and update the fields accordingly
            MsgToSend = new SendMessageData();
            MsgToSend.Type = MessageType.File;
            MsgToSend.Message = message;
            MsgToSend.ReplyMsgId = replyMsgId;
            MsgToSend.ReplyThreadId = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

            // Empty, as its a broadcast message
            MsgToSend.ReceiverIds = new int[] { };

            if (!Testing)
            {
                Trace.WriteLine("[UX] Sending file message");
                _model.CSend(MsgToSend);
            }
        }

        /// <summary>
        ///     Inform content module that a message is starred.
        /// </summary>
        /// <param name="msgId"> The message id of the message </param>
        public void StarChat(int msgId)
        {
            Trace.WriteLine("[UX] Message starred");
            _model.CMarkStar(msgId);
        }

        /// <summary>
        ///     Inform content module that a file needs to be downloaded.
        /// </summary>
        /// <param name="msgId"> The message id of the message </param>
        /// <param name="path"> The path to store the downloaded file in </param>
        public void DownloadFile(int msgId, string path)
        {
            Trace.WriteLine("[UX] File requested for download");
            _model.CDownload(msgId, path);
        }

        /// <summary>
        ///     Handles the property changed event raised on a component.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}