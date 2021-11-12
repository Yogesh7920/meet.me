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