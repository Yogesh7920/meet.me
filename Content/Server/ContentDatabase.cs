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
        private IMongoCollection<ChatContext> chatContexts;

        public ContentDatabase()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            databaseBase = mongoClient.GetDatabase("test");
            messages = databaseBase.GetCollection<MessageData>("messages");
            chatContexts = databaseBase.GetCollection<ChatContext>("chatContext");
        }

        public ObjectId Store(MessageData messageData)
        {
            messages.InsertOne(messageData);
            return messageData.MessageId;
        }

        public void Store(ChatContext chatContext)
        {
            chatContexts.InsertOne(chatContext);
        }

        public void UpdateChatContext(int id, ChatContext chatContext)
        {
            var filter = Builders<ChatContext>.Filter.Eq("ThreadId", id);
            chatContexts.ReplaceOne(filter, chatContext);
        }

        public MessageData RetrieveMessage(ObjectId messageId)
        {
            MessageData receiveMessageData = messages.Find(message => message.MessageId == messageId).FirstOrDefault();
            return receiveMessageData;
        }

        public List<MessageData> RetrieveMessage()
        {
            return messages.Find(message => true).ToList();
        }

        public List<ChatContext> RetrieveChatContexts()
        {
            return chatContexts.Find(chatContexts => true).ToList();
        }

    }
}