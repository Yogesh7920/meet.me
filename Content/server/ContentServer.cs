using System.Collections.Generic;

namespace Content
{
    internal class ContentServer : IContentServer
    {
        private List<IContentListener> subscribers;
        private List<Thread> allMessages;

        /// <summary>
        /// Add a new subscriber to the list of subscribers
        /// </summary>
        /// <param name="subscriber">IContentListener implementation provided by the subscriber</param>
        public void SSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <summary>
        /// Get all the messages sent
        /// </summary>
        /// <returns>List of Thread objects</returns>
        public List<Thread> SGetAllMessages()
        {
            return allMessages;
        }

        /// <summary>
        /// Sends all the messages to the client of the user with user id = userId
        /// </summary>
        /// <param name="userId">user id of the user to which messages needs to be sent</param>
        public void SSendAllMessagesToClient(int userId)
        {
            return;
        }
    }
}