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
            contentDatabase = new ContentDatabase();
            notificationHandler = new ContentServerNotificationHandler();
            fileServer = new FileServer();
            chatServer = new ChatServer(contentDatabase);
            serializer = new Serializer();
            communicator.Subscribe("ContentServer", notificationHandler);
        }

        public void Receive(string data)
        {
            MessageData messageData = serializer.Deserialize<MessageData>(data);
            ReceiveMessageData receiveMessageData = null;

            switch (messageData.Type)
            {
                case MessageType.Chat:
                    receiveMessageData = chatServer.Receive(messageData);
                    break;

                case MessageType.File:
                    fileServer.Receive(messageData);
                    break;

                default:
                    throw new System.Exception();
            }

            foreach (ChatContext chatContext in allMessages)
            {
                if (receiveMessageData != null && receiveMessageData.ReplyThreadId == chatContext.ThreadId)
                {
                    chatContext.MsgList.Add(receiveMessageData);
                    break;
                }
            }

            foreach (IContentListener subscriber in subscribers)
            {
                subscriber.OnMessage(receiveMessageData);
            }

            string message = serializer.Serialize<MessageData>(messageData);
            if (messageData.ReceiverIds.Length == 0)
            {
                communicator.Send(message, "Content");
            }
            else
            {
                foreach (string userId in messageData.ReceiverIds)
                {
                    communicator.Send(message, "Content", userId);
                }
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
        public void SSendAllMessagesToClient(string userId)
        {
            string allMessagesSerialized = serializer.Serialize<List<ChatContext>>(allMessages);
            communicator.Send(allMessagesSerialized, "Content", userId);
        }
    }
}