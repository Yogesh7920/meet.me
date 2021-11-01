using MongoDB.Bson;
using System.Collections.Generic;

namespace Content
{
    internal class ChatServer
    {
        private ContentDatabase contentDatabase;

        public ChatServer(ContentDatabase contentDatabase)
        {
            this.contentDatabase = contentDatabase;
        }

        public ObjectId Receive(MessageData messageData, List<ChatContext> allMessages)
        {
            ObjectId messageId = contentDatabase.Store(messageData);
            foreach (var chatConext in allMessages)
            {
                if (messageData.ReplyThreadId == chatConext.ThreadId)
                {
                    chatConext.MsgList.Add(messageData);
                }
            }
            return messageId;
        }

        public MessageData Fetch(ObjectId messageId)
        {
            return contentDatabase.RetrieveMessage(messageId);
        }
    }
}