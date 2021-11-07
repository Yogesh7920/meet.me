using Networking;
using System.Collections.Generic;
using System.Diagnostics;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> _subscribers;
        private ICommunicator _communicator;
        private INotificationHandler _notificationHandler;
        private ContentDatabase _contentDatabase;
        private ISerializer _serializer;
        private FileServer _fileServer;
        private ChatServer _chatServer;
        private ChatContextServer _chatContextServer;

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

        public void Receive(string data)
        {
            MessageData messageData = _serializer.Deserialize<MessageData>(data);
            MessageData receiveMessageData = null;

            Debug.Assert(messageData != null, "[ContentServer] Received null from Deserializer");

            switch (messageData.Type)
            {
                case MessageType.Chat:
                    receiveMessageData = _chatServer.Receive(messageData);
                    break;

                case MessageType.File:
                    receiveMessageData = _fileServer.Receive(messageData);
                    break;

                default:
                    Debug.Assert(false, "[ContentServer] Unknown Message Type");
                    break;
            }

            Debug.Assert(receiveMessageData != null, "[ContentServer] null returned by ChatServer/FileServer");

            if (messageData.Event != MessageEvent.Download)
            {
                _chatContextServer.Receive(receiveMessageData);
                Notify(messageData);
                Send(messageData);
            }
            else
            {
                // store file path on which the file will be downloaded on the client's system
                receiveMessageData.Message = messageData.Message;
                SendFile(receiveMessageData);
            }
        }

        private void Send(MessageData messageData)
        {
            string message = _serializer.Serialize<MessageData>(messageData);
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
            }
        }

        private void SendFile(MessageData messageData)
        {
            string message = _serializer.Serialize<MessageData>(messageData);
            _communicator.Send(message, "Content", messageData.SenderId.ToString());
        }

        private void Notify(ReceiveMessageData receiveMessageData)
        {
            foreach (IContentListener subscriber in _subscribers)
            {
                subscriber.OnMessage(receiveMessageData);
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
            return _chatContextServer.GetAllMessages();
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            string allMessagesSerialized = _serializer.Serialize<List<ChatContext>>(_chatContextServer.GetAllMessages());
            _communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }
    }
}