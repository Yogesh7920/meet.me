using MongoDB.Driver;

namespace Content
{
    internal class ContentDatabase
    {
        private IMongoClient mongoClient;
        private IMongoDatabase databaseBase;
        private IMongoCollection<ReceiveMessageData> messages;

        public ContentDatabase()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            databaseBase = mongoClient.GetDatabase("test");
            messages = databaseBase.GetCollection<ReceiveMessageData>("messages");
        }

        public void Store(ReceiveMessageData messageData)
        {
            messages.InsertOne(messageData);
        }

        public ReceiveMessageData Retrieve(int messageId)
        {
            ReceiveMessageData receiveMessageData = messages.Find<ReceiveMessageData>(message => message.MessageId == messageId).FirstOrDefault();
            return receiveMessageData;
        }
    }
}