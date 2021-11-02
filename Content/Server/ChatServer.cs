using MongoDB.Bson;

namespace Content
{
    internal class ChatServer
    {
        private ContentDatabase contentDatabase;

        public ChatServer(ContentDatabase contentDatabase)
        {
            this.contentDatabase = contentDatabase;
        }

        public MessageData Receive(MessageData messageData)
        {
            return contentDatabase.Store(messageData);
        }

        public MessageData Fetch(ObjectId messageId)
        {
            return contentDatabase.RetrieveMessage(messageId);
        }
    }
}