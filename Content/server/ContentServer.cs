using Networking;
using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private List<ChatContext> allMessages;
        private ICommunicator communicator;
        private INotificationHandler notificationHandler;
        private ContentDatabase contentDatabase;
        private ISerializer serializer;
        private FileServer fileServer;
        private ChatServer chatServer;

        public ContentServer()
        {
            subscribers = new List<IContentListener>();
            allMessages = new List<ChatContext>();
            communicator = CommunicationFactory.GetCommunicator();
            notificationHandler = new ContentServerNotificationHandler();
            fileServer = new FileServer();
            chatServer = new ChatServer();
            serializer = new Serializer();
            communicator.Subscribe("ContentServer", notificationHandler);
        }

        public void Receive(string data)
        {
            string type = serializer.GetObjectType(data, "Content");

            MessageData messageData = serializer.Deserialize<MessageData>(data);

            if (messageData.Type == MessageType.Chat)
            {
                chatServer.Receive(messageData);
            }
            else if (messageData.Type == MessageType.File)
            {
                fileServer.Receive(messageData);
            }
            else
            {
                throw new System.Exception();
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
            return allMessages;
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(int userId)
        {
            return;
        }
    }
}