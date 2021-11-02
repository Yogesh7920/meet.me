using Networking;
using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private List<MessageData> allMessages;
        private ICommunicator communicator;
        private INotificationHandler notificationHandler;
        private ContentDatabase contentDatabase;
        private ISerializer serializer;
        private FileServer fileServer;
        private ChatServer chatServer;

        public ContentServer()
        {
            subscribers = new List<IContentListener>();
            allMessages = new List<MessageData>();
            communicator = CommunicationFactory.GetCommunicator();
            contentDatabase = new ContentDatabase();
            notificationHandler = new ContentServerNotificationHandler();
            fileServer = new FileServer();
            chatServer = new ChatServer(contentDatabase);
            serializer = new Serializer();
            communicator.Subscribe("ContentServer", notificationHandler);
        }

        public void Receive(string data)
        {
            string type = serializer.GetObjectType(data, "Content");

            MessageData messageData = serializer.Deserialize<MessageData>(data);

            if (messageData.Type == MessageType.Chat)
            {
                allMessages.Add(chatServer.Receive(messageData));
            }
            else if (messageData.Type == MessageType.File)
            {
                fileServer.Receive(messageData);
            }
            else
            {
                throw new System.Exception();
            }

            foreach (IContentListener subscriber in subscribers)
            {
                subscriber.OnMessage(messageData);
            }

            string message = serializer.Serialize<MessageData>(messageData);
            communicator.Send(message, "Content");
        }

        /// <inheritdoc />
        public void SSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<MessageData> SGetAllMessages()
        {
            return allMessages;
        }

        /// <inheritdoc />
        public void SSendAllMessagesToClient(string userId)
        {
            string allMessagesSerialized = serializer.Serialize<List<MessageData>>(allMessages);
            communicator.Send(allMessagesSerialized, "Content", userId);
        }
    }
}