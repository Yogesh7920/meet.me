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

        public ContentServer()
        {
            subscribers = new List<IContentListener>();
            allMessages = new List<ChatContext>();
            communicator = CommunicationFactory.GetCommunicator();
            notificationHandler = new ContentServerNotificationHandler();
            communicator.Subscribe("ContentServer", notificationHandler);
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