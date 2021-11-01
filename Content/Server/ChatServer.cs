using MongoDB.Bson;

namespace Content
{
    internal class ChatServer
    {
        public void Receive(MessageData messageData)
        {

        }
        private ObjectId SaveChat(MessageData messageData, ContentDatabase contentDatabse)
        {
            return contentDatabse.Store(messageData);
        }

        private MessageData FetchChat(ObjectId messageId, ContentDatabase contentDatabse)
        {
            return contentDatabse.Retrieve(messageId);
        }
    }
}