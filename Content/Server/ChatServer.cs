using MongoDB.Bson;
using System;

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
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    return Store(messageData);

                case MessageEvent.Star:
                    return Star(messageData.MessageId);

                case MessageEvent.Update:
                    return Update(messageData.MessageId, messageData);

                default:
                    throw new NotImplementedException();
            }
        }

        private MessageData Update(ObjectId id, MessageData messageData)
        {
            contentDatabase.UpdateMessageData(id, messageData);
            return messageData;
        }

        private MessageData Star(ObjectId id)
        {
            MessageData messageData = Fetch(id);
            messageData.Starred = !messageData.Starred;
            contentDatabase.UpdateMessageData(id, messageData);
            return messageData;
        }

        private MessageData Store(MessageData messageData)
        {
            return contentDatabase.Store(messageData);
        }

        private MessageData Fetch(ObjectId messageId)
        {
            return contentDatabase.RetrieveMessage(messageId);
        }
    }
}