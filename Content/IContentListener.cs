using System.Collections.Generic;

namespace Content
{
    public interface IContentListener
    {
        /// <summary>
        /// Handler for messages received by the Content module.
        /// </summary>
        /// <param name="messageData">Received message</param>
        void OnMessage(ReceiveMessageData messageData);

        /// <summary>
        /// Handler for the event of all messages sent to/from client being received at once
        /// The Dashboard module may simply throw an excpetion in the body of
        /// this function because it doesn't expect to receive list of all messages
        /// as it is running on the server, not on the clients.
        /// </summary>
        /// <param name="allMessages">list of Thread objects containing all messages</param>
        void OnAllMessages(List<ChatContext> allMessages);
    }
}