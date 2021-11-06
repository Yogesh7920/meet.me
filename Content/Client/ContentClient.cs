using System;
using System.Collections.Generic;
using Networking;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private int userId;
        public int UserId 
        { 
            get => userId; 
            set
            {
                userId = value;
                fileHandler.UserId = value;
                // chatHandler.UserId = value;
            }
        }

        private List<IContentListener> subscribers;
        private Queue<SendMessageData> sendQueue;
        private List<ChatContext> allMessages;
        Dictionary<int, int> threadMap;

        private FileClient fileHandler;
        // private ChatClient chatHandler;
        private INotificationHandler notifHandler;
        private ICommunicator communicator;

        public ContentClient()
        {
            userId = -1;
            subscribers = new List<IContentListener>();
            sendQueue = new Queue<SendMessageData>();
            allMessages = new List<ChatContext>();
            threadMap = new Dictionary<int, int>();
            fileHandler = new FileClient();
            // chatHandler = new ChatClient();
            
            // subscribe to the network
            notifHandler = new ContentClientNotificationHandler();
            communicator.Subscribe()
        }

        /// <inheritdoc/>
        public void CSend(SendMessageData toSend)
        {
            switch(toSend.Type)
            {
                case MessageType.Chat:
                    // chatHandler.Send(toSend);
                    break;
                
                case MessageType.File:
                    fileHandler.Send(toSend);
                    break;

                default:
                    throw new ArgumentException("Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File");

            }
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

        
        public void Notify(ReceiveMessageData message)
        {
            foreach (IContentListener subscriber in subscribers)
            {
                subscriber.OnMessage(message);
            }
        }
    }
}