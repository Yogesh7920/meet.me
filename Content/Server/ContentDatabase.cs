using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Content
{
    internal class ContentDatabase
    {
        private IMongoClient mongoClient;
        private IMongoDatabase databaseBase;
        private IMongoCollection<MessageData> messages;

        public ContentDatabase()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            databaseBase = mongoClient.GetDatabase("test");
            messages = databaseBase.GetCollection<MessageData>("messages");
        }

        public ObjectId Store(MessageData messageData)
        {
            messages.InsertOne(messageData);
            return messageData.MessageId;
        }

        public MessageData Retrieve(ObjectId messageId)
        {
            MessageData receiveMessageData = messages.Find(message => message.MessageId == messageId).FirstOrDefault();
            return receiveMessageData;
        }

        public List<MessageData> Retrieve()
        {
            return messages.Find( message => true).ToList();
        }
    }
}