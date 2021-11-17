using System.Collections.Generic;
using System.Diagnostics;
using Networking;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private readonly ChatContextServer _chatContextServer;
        private readonly ChatServer _chatServer;
        private readonly ICommunicator _communicator;
        private readonly ContentDatabase _contentDatabase;
        private readonly FileServer _fileServer;
        private readonly INotificationHandler _notificationHandler;
        private readonly ISerializer _serializer;
        private readonly List<IContentListener> _subscribers;

        public ContentServer()
        {
            _subscribers = new List<IContentListener>();
            _communicator = CommunicationFactory.GetCommunicator(false);
            _contentDatabase = new ContentDatabase();
            _notificationHandler = new ContentServerNotificationHandler();
            _fileServer = new FileServer(_contentDatabase);
            _chatServer = new ChatServer(_contentDatabase);
            _chatContextServer = new ChatContextServer(_contentDatabase);
            _serializer = new Serializer();
            _communicator.Subscribe("Content", _notificationHandler);
        }

        /// <inheritdoc />
        public void SSubscribe(IContentListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<ChatContext> SGetAllMessages()
        {
            return _chatContextServer.GetAllMessages();
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            var allMessagesSerialized = _serializer.Serialize(_chatContextServer.GetAllMessages());
            _communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }

        public void Receive(string data)
        {
            var messageData = _serializer.Deserialize<MessageData>(data);
            MessageData receiveMessageData;

            Trace.WriteLine("[ContentServer] Received messageData from ContentServerNotificationHandler");
            Debug.Assert(messageData != null, "[ContentServer] Received null from Deserializer");

            switch (messageData.Type)
            {
                case MessageType.Chat:
                    Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                    receiveMessageData = _chatServer.Receive(messageData);
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

            if (messageData.Event != MessageEvent.Download)
            {
                Trace.WriteLine("[ContentServer] Event is " + messageData.Event);
                Trace.WriteLine("[ContentServer] Updating ChatContext");
                _chatContextServer.Receive(receiveMessageData);
                Trace.WriteLine("[ContentServer] Notifying subscribers");
                Notify(messageData);
                Trace.WriteLine("[ContentServer] Sending message to clients");
                Send(messageData);
            }
            else
            {
                Trace.WriteLine("[ContentServer] Event is Download");
                // store file path on which the file will be downloaded on the client's system
                receiveMessageData.Message = messageData.Message;
                Trace.WriteLine("[ContentServer] Sending File to client");
                SendFile(receiveMessageData);
            }

            Trace.WriteLine("[ContentServer] Message sent");
        }

        private void Send(MessageData messageData)
        {
            var message = _serializer.Serialize(messageData);
            if (messageData.ReceiverIds.Length == 0)
                _communicator.Send(message, "Content");
            else
                foreach (var userId in messageData.ReceiverIds)
                    _communicator.Send(message, "Content", userId.ToString());
        }

        private void SendFile(MessageData messageData)
        {
            var message = _serializer.Serialize(messageData);
            _communicator.Send(message, "Content", messageData.SenderId.ToString());
        }

        private void Notify(ReceiveMessageData receiveMessageData)
        {
            foreach (var subscriber in _subscribers) subscriber.OnMessage(receiveMessageData);
        }
    }
}