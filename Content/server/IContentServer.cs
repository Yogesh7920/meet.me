using System.Collections.Generic;

namespace Content
{
    public interface IContentServer
    {
        /// <summary>
        /// Add a new subscriber to the list of subscribers
        /// </summary>
        /// <param name="subscriber">IContentListener implementation provided by the subscriber</param>
        void SSubscribe(IContentListener subscriber);

        /// <summary>
        /// Get all the messages sent
        /// </summary>
        /// <returns>List of Thread objects</returns>
        List<Thread> SGetAllMessages();

        /// <summary>
        /// Sends all the messages to the client of the user with user id = userId
        /// </summary>
        /// <param name="userId">user id of the user to which messages needs to be sent</param>
        void SSendAllMessagesToClient(int userId);
    }
}