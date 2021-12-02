/// <author>Sameer Dhiman</author>
/// <created>18/10/2021</created>
/// <summary>
///     This file handles all the messages that come to server (files and chats)
///     and passes them to their respective classes
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Networking;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private static readonly object _lock = new();
        private readonly INotificationHandler _notificationHandler;
        private ChatContextServer _chatContextServer;
        private ICommunicator _communicator;
        private ContentDatabase _contentDatabase;
        private FileServer _fileServer;
        private ISerializer _serializer;
        private List<IContentListener> _subscribers;

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

        /// <summary>
        ///     Get and Set Communicator, Meant to be only used for testing
        /// </summary>
        internal ICommunicator Communicator
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
            var allMessagesSerialized = _serializer.Serialize(SGetAllMessages());
            _communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }

        /// <summary>
        ///     Receives data from ContentServerNotificationHandler and processes it accordingly
        /// </summary>
        /// <param name="data"></param>
        public void Receive(string data)
        {
            MessageData messageData;
            // Try deserializing the data if error then do nothing and return.
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

            // lock to prevent multiple threads from modifying the messages at once.
            lock (_lock)
            {
                switch (messageData.Type)
                {
                    case MessageType.Chat:
                        Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                        receiveMessageData = _chatContextServer.Receive(messageData);
                        break;

                    case MessageType.File:
                        Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                        receiveMessageData = _fileServer.Receive(messageData);
                        break;

                    case MessageType.HistoryRequest:
                        Trace.WriteLine(
                            "[ContentServer] MessageType is HistoryRequest, Calling ContentServer.SSendAllMessagesToClient");
                        SSendAllMessagesToClient(messageData.SenderId);
                        return;

                    default:
                        Trace.WriteLine("[ContentServer] Unknown Message Type");
                        return;
                }
            }

            // If this is null then something went wrong, probably message was not found.
            if (receiveMessageData == null)
            {
                Trace.WriteLine("[ContentServer] Something went wrong while handling the message.");
                return;
            }

            try
            {
                // If Event is Download then send the file to client
                if (messageData.Event == MessageEvent.Download)
                {
                    Trace.WriteLine("[ContentServer] Sending File to client");
                    SendFile(receiveMessageData);
                }
                // Else send the message to all the receivers and notify the subscribers
                else
                {
                    Trace.WriteLine("[ContentServer] Notifying subscribers");
                    Notify(receiveMessageData);
                    Trace.WriteLine("[ContentServer] Sending message to clients");
                    Send(receiveMessageData);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Something went wrong while sending message. Exception {e}");
                return;
            }

            Trace.WriteLine("[ContentServer] Message sent");
        }

        /// <summary>
        ///     Sends the message to clients.
        /// </summary>
        /// <param name="messageData"></param>
        private void Send(MessageData messageData)
        {
            var message = _serializer.Serialize(messageData);

            // If length of ReceiverIds is 0 that means its a broadcast.
            if (messageData.ReceiverIds.Length == 0)
            {
                _communicator.Send(message, "Content");
            }
            // Else send the message to the receivers in ReceiversIds.
            else
            {
                foreach (var userId in messageData.ReceiverIds)
                    _communicator.Send(message, "Content", userId.ToString());
                // Sending the message back to the sender.
                _communicator.Send(message, "Content", messageData.SenderId.ToString());
            }
        }

        /// <summary>
        ///     Sends the file back to the requester.
        /// </summary>
        /// <param name="messageData"></param>
        private void SendFile(MessageData messageData)
        {
            var message = _serializer.Serialize(messageData);
            _communicator.Send(message, "Content", messageData.SenderId.ToString());
        }

        /// <summary>
        ///     Notifies all the subscribed modules.
        /// </summary>
        /// <param name="receiveMessageData"></param>
        private void Notify(ReceiveMessageData receiveMessageData)
        {
            foreach (var subscriber in _subscribers) _ = Task.Run(() => { subscriber.OnMessage(receiveMessageData); });
        }

        /// <summary>
        ///     Resets the ContentServer, Meant to be used only for Testing
        /// </summary>
        internal void Reset()
        {
            _subscribers = new List<IContentListener>();
            _contentDatabase = new ContentDatabase();
            _fileServer = new FileServer(_contentDatabase);
            _chatContextServer = new ChatContextServer(_contentDatabase);
            _serializer = new Serializer();
        }
    }
}