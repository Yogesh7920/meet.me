using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private List<Thread> allMessages;

        /// <inheritdoc />
        public void SSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<Thread> SGetAllMessages()
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