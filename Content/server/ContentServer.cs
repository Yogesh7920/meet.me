using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private List<Thread> allMessages;

        public void SSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        public List<Thread> SGetAllMessages()
        {
            return allMessages;
        }

        public void SSendAllMessagesToClient(int userId)
        {
            return;
        }
    }
}