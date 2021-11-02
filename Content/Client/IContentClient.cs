namespace Content
{
    public interface IContentClient
    {
        /// <summary>
        /// Sends chat or file message to specified clients
        /// </summary>
        /// <param name="toSend">Message to send. In case of file, toSend.message should contain file path</param>
        void CSend(SendMessageData toSend);

        /// <summary>
        /// Download a file message to specified file path on the client machine
        /// </summary>
        /// <param name="messageId">Message Id corresponding to the file to be downloaded</param>
        /// <param name="savePath">Path to which the downloaded file will be saved</param>
        void CDownload(int messageId, string savePath);

        /// <summary>
        /// Star a message which prioritises it to be included in dashboard summary
        /// </summary>
        /// <param name="messageId">Message Id of message to be starred</param>
        void CMarkStar(int messageId);

        /// <summary>
        /// Update a previously sent chat message
        /// </summary>
        /// <param name="messageId">Messsage Id of the chat message to be updated</param>
        /// <param name="newMessage">New updated chat message</param>
        void CUpdateChat(int messageId, string newMessage);

        /// <summary>
        /// Subscribe to content module for listening to received messages
        /// </summary>
        /// <param name="subscriber">An implementation of the IContentListener interface</param>
        void CSubscribe(IContentListener subscriber);

        /// <summary>
        /// Get the thread corresponding to a thread id
        /// </summary>
        /// <param name="threadId">Id of the requested thread</param>
        /// <returns>Thread object corresponding to specified thread Id</returns>
        ChatContext CGetThread(int threadId);
    }
}