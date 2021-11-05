using Networking;
using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private ICommunicator communicator;
        private INotificationHandler notificationHandler;
        private ContentDatabase contentDatabase;
        private ISerializer serializer;
        private FileServer fileServer;
        private ChatServer chatServer;
        private ChatContextServer chatContextServer;

        public ContentServer()
        {
            subscribers = new List<IContentListener>();
            communicator = CommunicationFactory.GetCommunicator();
            contentDatabase = new ContentDatabase();
            notificationHandler = new ContentServerNotificationHandler();
            fileServer = new FileServer();
            chatServer = new ChatServer(contentDatabase);
            chatContextServer = new ChatContextServer(contentDatabase);
            serializer = new Serializer();
            communicator.Subscribe("ContentServer", notificationHandler);
        }

        public void Receive(string data)
        {
            MessageData messageData = serializer.Deserialize<MessageData>(data);
            MessageData receiveMessageData = null;

            switch (messageData.Type)
            {
                case MessageType.Chat:
                    receiveMessageData = (MessageData)chatServer.Receive(messageData);
                    break;

                case MessageType.File:
                    fileServer.Receive(messageData);
                    break;

                default:
                    throw new System.Exception();
            }

            if (messageData.Event != MessageEvent.Download)
            {
                chatContextServer.Receive(receiveMessageData);
                Noyify(messageData);
            }

            Send(messageData);
        }

        private void Send(MessageData messageData)
        {
            string message = serializer.Serialize<MessageData>(messageData);
            if (messageData.ReceiverIds.Length == 0)
            {
                communicator.Send(message, "Content");
            }
            else
            {
                foreach (int userId in messageData.ReceiverIds)
                {
                    communicator.Send(message, "Content", userId.ToString());
                }
            }
        }

        private void Noyify(ReceiveMessageData receiveMessageData)
        {
            foreach (IContentListener subscriber in subscribers)
            {
                subscriber.OnMessage(receiveMessageData);
            }
        }

        /// <inheritdoc />
        public void SSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<ChatContext> SGetAllMessages()
        {
            return chatContextServer.GetAllMessages();
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            string allMessagesSerialized = serializer.Serialize<List<ChatContext>>(chatContextServer.GetAllMessages());
            communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }
    }
}