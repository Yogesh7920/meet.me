using System;
using System.IO;
using System.Collections.Generic;
using Networking;
using MongoDB.Bson;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private int _userId;
        public int UserId 
        { 
            get => _userId; 
            set
            {
                _userId = value;
                _fileHandler.UserId = value;
                _chatHandler.UserId = value;
            }
        }

        private List<IContentListener> _subscribers;
        private List<ChatContext> _allMessages;
        private Dictionary<ObjectId, int> _contextMap;

        // dictionary that maps message events to their respective handler functions
        private Dictionary<MessageEvent, Action<MessageData> > _messageHandlers;

        private FileClient _fileHandler;
        private ChatClient _chatHandler;
        private INotificationHandler _notifHandler;
        private ICommunicator _communicator;

        public ContentClient()
        {
            _userId = -1;
            _subscribers = new List<IContentListener>();
            _allMessages = new List<ChatContext>();
            _contextMap = new Dictionary<ObjectId, int>();
            _fileHandler = new FileClient();
            _chatHandler = new ChatClient();

            // add message handler functions for each event
            _messageHandlers = new Dictionary<MessageEvent, Action<MessageData>>();
            _messageHandlers[MessageEvent.NewMessage] = new Action<MessageData>(NewMessageHandler);
            _messageHandlers[MessageEvent.Update] = new Action<MessageData>(UpdateMessageHandler);
            _messageHandlers[MessageEvent.Star] = new Action<MessageData>(StarMessageHandler);
            _messageHandlers[MessageEvent.Download] = new Action<MessageData>(DownloadMessageHandler);

            // subscribe to the network
            _notifHandler = new ContentClientNotificationHandler();
            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe("Content", _notifHandler);

        }

        /// <inheritdoc/>
        public void CSend(SendMessageData toSend)
        {
            switch(toSend.Type)
            {
                case MessageType.Chat:
                    _chatHandler.ChatNewMessage(toSend);
                    break;
                
                case MessageType.File:
                    _fileHandler.Send(toSend);
                    break;

                default:
                    throw new ArgumentException("Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File");

            }
        }

        /// <inheritdoc/>
        public void CDownload(ObjectId messageId, string savePath)
        {
            // check that the message with ID messageID exists and is indeed a file
            int found = 0;
            foreach (ChatContext chatThread in _allMessages)
            {
                foreach (ReceiveMessageData msg in chatThread.MsgList)
                {
                    if (msg.MessageId == messageId)
                    {
                        found = 1;
                        if (msg.Type != MessageType.File)
                            throw new ArgumentException("Message requested for download is not a file type message");
                    }
                }
            }

            if (found == 0)
            {
                throw new ArgumentException("Message with given message ID not found");
            }

            _fileHandler.Download(messageId, savePath);
        }

        /// <inheritdoc/>
        public void CMarkStar(ObjectId messageId)
        {
            _chatHandler.ChatStar(messageId);
        }

        /// <inheritdoc/>
        public void CUpdateChat(ObjectId messageId, string newMessage)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CSubscribe(IContentListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc/>
        public ChatContext CGetThread(ObjectId threadId)
        {
            if (_contextMap.ContainsKey(threadId))
            {
                int index = _contextMap[threadId];
                return _allMessages[index];
            }
            else
            {
                throw new ArgumentException("Thread with requested thread ID does not exist");
            }
        }

        /// <summary>
        /// Notify all subscribers of received message
        /// </summary>
        /// <param name="message">The message object to call OnMessage function of subscribers with</param>
        public void Notify(ReceiveMessageData message)
        {
            foreach (IContentListener subscriber in _subscribers)
            {
                subscriber.OnMessage(message);
            }
        }

        
        /// <summary>
        /// Notify all subscribers of received entire message history
        /// </summary>
        /// <param name="allMessages">The entire message history to call OnAllMessages function of subscribers with</param>
        public void Notify(List<ChatContext> allMessages)
        {
            foreach (IContentListener subscriber in _subscribers)
            {
                subscriber.OnAllMessages(allMessages);
            }
        }

        
        /// <summary>
        /// Handles received messages from the network
        /// </summary>
        /// <param name="message">The received message</param>
        public void OnReceive(MessageData message)
        {
            _messageHandlers[message.Event](message);
        }
        
        public void NewMessageHandler(MessageData message)
        {
            // in case there is any file data associated with the message (there shouldn't be)
            // make file data null
            if (message.FileData != null)
            {
                message.FileData = null;
            }
            
            ReceiveMessageData receivedMessage = message as ReceiveMessageData;
            
            // add the message to the correct ChatContext in allMessages
            ObjectId key = receivedMessage.ReplyThreadId;
            if (_contextMap.ContainsKey(key))
            {
                int index = _contextMap[key];
                _allMessages[index].MsgList.Add(receivedMessage);
                _allMessages[index].NumOfMessages += 1;
            }
            else // in case the message is part of a new thread
            {
                // create new thread with given id
                ChatContext newContext = new ChatContext();
                newContext.ThreadId = key;
                newContext.MsgList.Add(receivedMessage);
                newContext.NumOfMessages = 1;
                newContext.CreationTime = receivedMessage.SentTime;

                _allMessages.Add(newContext);
                
                // add entry in the hash table to keep track of ChatContext with given thread Id
                int index = _allMessages.Count - 1;
                _contextMap.Add(key, index);
            }

            // notify the subscribers of the new message
            Notify(receivedMessage);
        }

        public void UpdateMessageHandler(MessageData message)
        {
            if (message.FileData != null)
            {
                message.FileData = null;
            }
            
            ReceiveMessageData receivedMessage = message as ReceiveMessageData;

            // update the message in _allMessages
            ObjectId key = receivedMessage.ReplyThreadId;
            if (_contextMap.ContainsKey(key))
            {
                int index = _contextMap[key];
                int numMessages = _allMessages[index].NumOfMessages;
                int i;
                for (i = 0; i < numMessages; i++)
                {
                    ObjectId id = _allMessages[index].MsgList[i].MessageId;
                    if (id == receivedMessage.MessageId)
                    {
                        _allMessages[index].MsgList[i] = receivedMessage;
                        break;
                    }
                }
                
                // if no match was found, there is an error
                if (i == numMessages)
                {
                    throw new ArgumentException("No message with given id exists");
                }
            }
            else
            {
                throw new ArgumentException("No message thread with given id exists");
            }

            // notify subscribers
            Notify(receivedMessage);
        }

        public void StarMessageHandler(MessageData message)
        {
            ObjectId contextId = message.ReplyThreadId;
            ObjectId messageId = message.MessageId;

            if (_contextMap.ContainsKey(contextId))
            {
                int index = _contextMap[contextId];
                int numMessages = _allMessages[index].NumOfMessages;
                int i;
                for (i = 0; i < numMessages; i++)
                {
                    ObjectId id = _allMessages[index].MsgList[i].MessageId;
                    if (id == messageId)
                    {
                        _allMessages[index].MsgList[i].Starred = true;
                        break;
                    }
                }
                
                // if no match was found, there is an error
                if (i == numMessages)
                {
                    throw new ArgumentException("No message with given id exists");
                }
            }
            else
            {
                throw new ArgumentException("No message thread with given id exists");
            }

            Notify(message as ReceiveMessageData);
        }

        public void DownloadMessageHandler(MessageData message)
        {
            string savedirpath = message.Message;
            string savepath = savedirpath + message.FileData.fileName;

            File.WriteAllBytes(savepath, message.FileData.fileContent);
        }
    }
}