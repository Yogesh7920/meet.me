using System;
using System.Collections.Generic;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private int userId;
        public int UserId { get => userId; set => userId = value; }
        
        private List<IContentListener> subscribers;
        private Queue<SendMessageData> sendQueue;
        private List<ChatContext> allMessages;
        Dictionary<int, int> threadMap;

        public ContentClient()
        {
            userId = -1;
            subscribers = new List<IContentListener>();
            sendQueue = new Queue<SendMessageData>();
            allMessages = new List<ChatContext>();
            threadMap = new Dictionary<int, int>();
        }

        /// <inheritdoc/>
        public void CSend(SendMessageData toSend)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CDownload(int messageId, string savePath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CMarkStar(int messageId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CUpdateChat(int messageId, string newMessage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CSubscribe(IContentListener subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <inheritdoc/>
        public ChatContext CGetThread(int threadId)
        {
            throw new NotImplementedException();
        }

    }
}