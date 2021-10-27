using MongoDB.Bson;
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

        public ObjectId Store(ReceiveMessageData messageData)
        {
            messages.InsertOne(messageData);
            return messageData.MessageId;
        }

        public ReceiveMessageData Retrieve(ObjectId messageId)
        {
            ReceiveMessageData receiveMessageData = messages.Find(message => message.MessageId == messageId).FirstOrDefault();
            return receiveMessageData;
        }
    }
}