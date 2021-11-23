using Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private readonly List<IContentListener> _subscribers;
        private ICommunicator _communicator;
        private readonly INotificationHandler _notificationHandler;
        private readonly ContentDatabase _contentDatabase;
        private readonly ISerializer _serializer;
        private readonly FileServer _fileServer;
        private readonly ChatContextServer _chatContextServer;
        private static readonly object _lock = new();

        public ContentServer()
        {
            _subscribers = new List<IContentListener>();
            _communicator = CommunicationFactory.GetCommunicator(false);
            _contentDatabase = new ContentDatabase();
            _notificationHandler = new ContentServerNotificationHandler(this);
            _fileServer = new FileServer(_contentDatabase);
            _chatContextServer = new ChatContextServer(_contentDatabase);
            _serializer = new Serializer();
            _communicator.Subscribe("Content", _notificationHandler);
        }

        // getter and setter for communicator
        public ICommunicator Communicator
        {
            get => _communicator;
            set
            {
                _communicator = value;
                _communicator.Subscribe("Content", _notificationHandler);
            }
        }

        /// <inheritdoc />
        public void SSubscribe(IContentListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<ChatContext> SGetAllMessages()
        {
            lock (_lock)
            {
                return _chatContextServer.GetAllMessages();
            }
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            string allMessagesSerialized = _serializer.Serialize(SGetAllMessages());
            _communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }

        /// <summary>
        /// This function convets MessageData to ReceiveMessageData for chatContexts
        /// </summary>
        /// <param name="msgData"></param>
        /// <returns></returns>
        public MessageData ReceiveMessageDataToMessageData(ReceiveMessageData msgData)
        {
            var msg = new MessageData();
            msg.Event = msgData.Event;
            msg.Message = msgData.Message;
            msg.MessageId = msgData.MessageId;
            msg.ReceiverIds = msgData.ReceiverIds;
            msg.SenderId = msgData.SenderId;
            msg.ReplyThreadId = msgData.ReplyThreadId;
            msg.Starred = msgData.Starred;
            msg.Type = msgData.Type;
            return msg;
        }

        /// <summary>
        /// Receives data from ContentServerNotificationHandler and processes it accordingly
        /// </summary>
        /// <param name="data"></param>
        public void Receive(string data)
        {
            MessageData messageData;
            try
            {
                messageData = _serializer.Deserialize<MessageData>(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception occured while deserialsing data. Exception: {e}");
                return;
            }
            MessageData receiveMessageData;

            Trace.WriteLine("[ContentServer] Received messageData from ContentServerNotificationHandler");
            Debug.Assert(messageData != null, "[ContentServer] Received null from Deserializer");

            lock (_lock)
            {
                switch (messageData.Type)
                {
                    case MessageType.Chat:
                        Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                        receiveMessageData = ReceiveMessageDataToMessageData(_chatContextServer.Receive(messageData));
                        Debug.Assert(receiveMessageData != null, "[ContentServer] null returned by ChatServer");
                        break;

                    case MessageType.File:
                        Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                        receiveMessageData = _fileServer.Receive(messageData);
                        Debug.Assert(receiveMessageData != null, "[ContentServer] null returned by FileServer");
                        break;

                    default:
                        Debug.Assert(false, "[ContentServer] Unknown Message Type");
                        return;
                }
            }

            if (receiveMessageData == null)
            {
                Trace.WriteLine("[ContentServer] Something went wrong while handling the message.");
                return;
            }

            if (messageData.Event != MessageEvent.Download)
            {
                Trace.WriteLine("[ContentServer] Notifying subscribers");
                Notify(receiveMessageData);
                Trace.WriteLine("[ContentServer] Sending message to clients");
                Send(receiveMessageData);
            }
            else
            {
                Trace.WriteLine("[ContentServer] Sending File to client");
                SendFile(receiveMessageData);
            }

            Trace.WriteLine("[ContentServer] Message sent");
        }

        /// <summary>
        /// Sends the message to clients.
        /// </summary>
        /// <param name="messageData"></param>
        private void Send(MessageData messageData)
        {
            string message = _serializer.Serialize(messageData);
            if (messageData.ReceiverIds.Length == 0)
            {
                _communicator.Send(message, "Content");
            }
            else
            {
                foreach (int userId in messageData.ReceiverIds)
                {
                    _communicator.Send(message, "Content", userId.ToString());
                }
                _communicator.Send(message, "Content", messageData.SenderId.ToString());
            }
        }

        /// <summary>
        /// Sends the file back to the requester.
        /// </summary>
        /// <param name="messageData"></param>
        private void SendFile(MessageData messageData)
        {
            string message = _serializer.Serialize(messageData);
            _communicator.Send(message, "Content", messageData.SenderId.ToString());
        }

        /// <summary>
        /// Notifies all the subscribed modules.
        /// </summary>
        /// <param name="receiveMessageData"></param>
        private void Notify(ReceiveMessageData receiveMessageData)
        {
            foreach (IContentListener subscriber in _subscribers)
            {
                subscriber.OnMessage(receiveMessageData);
            }
        }
    }
}