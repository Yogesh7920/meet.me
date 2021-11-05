using Networking;
using System.Collections.Generic;

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
            _communicator = CommunicationFactory.GetCommunicator();
            _contentDatabase = new ContentDatabase();
            _notificationHandler = new ContentServerNotificationHandler();
            _fileServer = new FileServer();
            _chatServer = new ChatServer(_contentDatabase);
            _chatContextServer = new ChatContextServer(_contentDatabase);
            _serializer = new Serializer();
            _communicator.Subscribe("ContentServer", _notificationHandler);
        }

        public void Receive(string data)
        {
            MessageData messageData = _serializer.Deserialize<MessageData>(data);
            MessageData receiveMessageData = null;

            switch (messageData.Type)
            {
                case MessageType.Chat:
                    receiveMessageData = (MessageData)_chatServer.Receive(messageData);
                    break;

                case MessageType.File:
                    _fileServer.Receive(messageData);
                    break;

                default:
                    throw new System.Exception();
            }

            if (messageData.Event != MessageEvent.Download)
            {
                _chatContextServer.Receive(receiveMessageData);
                Noyify(messageData);
            }

            Send(messageData);
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

        private void Noyify(ReceiveMessageData receiveMessageData)
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