using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Testing")]

namespace Content
{
    internal class ContentDatabase
    {
        private IMongoClient _mongoClient;
        private IMongoDatabase _databaseBase;
        private IMongoCollection<MessageData> _messages;
        private IMongoCollection<ChatContext> _chatContexts;

        public ContentDatabase()
        {
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _databaseBase = _mongoClient.GetDatabase("test");
            _messages = _databaseBase.GetCollection<MessageData>("messages");
            _chatContexts = _databaseBase.GetCollection<ChatContext>("chatContext");
        }

        public MessageData Store(MessageData messageData)
        {
            _messages.InsertOne(messageData);
            return messageData;
        }

        public void Store(ChatContext chatContext)
        {
            _chatContexts.InsertOne(chatContext);
        }

        public void UpdateMessageData(ObjectId id, MessageData messageData)
        {
            var filter = Builders<MessageData>.Filter.Eq("MessageId", id);
            _messages.ReplaceOne(filter, messageData);
        }

        public void UpdateChatContext(ObjectId id, ChatContext chatContext)
        {
            var filter = Builders<ChatContext>.Filter.Eq("ThreadId", id);
            _chatContexts.ReplaceOne(filter, chatContext);
        }

        public MessageData RetrieveMessage(ObjectId messageId)
        {
            MessageData receiveMessageData = _messages.Find(message => message.MessageId == messageId).FirstOrDefault();
            return receiveMessageData;
        }

        public List<MessageData> RetrieveMessage()
        {
            return _messages.Find(message => true).ToList();
        }

        public List<ChatContext> RetrieveChatContexts()
        {
            return _chatContexts.Find(chatContexts => true).ToList();
        }
    }
}