/// <author>Yuvraj Raghuvanshi</author>
/// <created>16/10/2021</created>
using Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private readonly List<ChatContext> _allMessages;
        private readonly ChatClient _chatHandler;

        private ICommunicator _communicator;

        private readonly Dictionary<int, int> _contextMap;

        private readonly FileClient _fileHandler;

        // dictionary that maps message events to their respective handler functions
        private readonly Dictionary<MessageEvent, Action<MessageData>> _messageHandlers;
        private readonly INotificationHandler _notifHandler;

        private readonly List<IContentListener> _subscribers;
        private int _userId;

        public ContentClient()
        {
            _userId = -1;
            _subscribers = new List<IContentListener>();
            _allMessages = new List<ChatContext>();
            _contextMap = new Dictionary<int, int>();

            // add message handler functions for each event
            _messageHandlers = new Dictionary<MessageEvent, Action<MessageData>>();
            _messageHandlers[MessageEvent.NewMessage] = NewMessageHandler;
            _messageHandlers[MessageEvent.Update] = UpdateMessageHandler;
            _messageHandlers[MessageEvent.Star] = StarMessageHandler;
            _messageHandlers[MessageEvent.Download] = DownloadMessageHandler;

            // subscribe to the network
            _notifHandler = new ContentClientNotificationHandler(this);
            _communicator = CommunicationFactory.GetCommunicator();
            _communicator.Subscribe("Content", _notifHandler);

            // initialize file handler and chat handler
            _fileHandler = new FileClient(_communicator);
            _chatHandler = new ChatClient(_communicator);
        }

        // getter and setter for communicator
        public ICommunicator Communicator
        {
            get => _communicator;
            set
            {
                _communicator = value;
                _fileHandler.Communicator = value;
                _chatHandler.Communicator = value;
            }
        }

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

        /// <inheritdoc />
        public void CSend(SendMessageData toSend)
        {
            switch (toSend.Type)
            {
                case MessageType.Chat:
                    Trace.WriteLine("[ContentClient] Sending chat message");
                    _chatHandler.ChatNewMessage(toSend);
                    break;

                case MessageType.File:
                    Trace.WriteLine("[ContentClient] Sending file message");
                    _fileHandler.Send(toSend);
                    break;

                default:
                    throw new ArgumentException(
                        "Invalid MessageType field. Must be one of MessageType.Chat or MessageType.File");
            }
        }

        /// <inheritdoc />
        public void CDownload(int messageId, string savePath)
        {
            // check that the message with ID messageID exists and is indeed a file
            var found = 0;
            foreach (var chatThread in _allMessages)
            {
                foreach (var msg in chatThread.MsgList)
                    if (msg.MessageId == messageId)
                    {
                        found = 1;
                        if (msg.Type != MessageType.File)
                            throw new ArgumentException("Message requested for download is not a file type message");
                        break;
                    }

                if (found == 1)
                    break;
            }

            if (found == 0)
            {
                Trace.WriteLine("[ContentClient] File requested for download not found");
                throw new ArgumentException("Message with given message ID not found");
            }

            _fileHandler.Download(messageId, savePath);
        }

        /// <inheritdoc />
        public void CMarkStar(int messageId)
        {
            _chatHandler.ChatStar(messageId);
        }

        /// <inheritdoc />
        public void CUpdateChat(int messageId, string newMessage)
        {
            _chatHandler.ChatUpdate(messageId, newMessage);
        }

        /// <inheritdoc />
        public void CSubscribe(IContentListener subscriber)
        {
            Trace.WriteLine("[ContentClient] Added new subscriber");
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public ChatContext CGetThread(int threadId)
        {
            if (_contextMap.ContainsKey(threadId))
            {
                var index = _contextMap[threadId];
                Trace.WriteLine($"[ContentClient] Returning thread with id {threadId}");
                return _allMessages[index];
            }

            throw new ArgumentException("Thread with requested thread ID does not exist");
        }

        /// <summary>
        ///     Notify all subscribers of received message
        /// </summary>
        /// <param name="message">The message object to call OnMessage function of subscribers with</param>
        public void Notify(ReceiveMessageData message)
        {
            Trace.WriteLine("[ContentClient] Notifying subscribers of new received message");
            foreach (var subscriber in _subscribers) subscriber.OnMessage(message);
        }

        /// <summary>
        ///     Notify all subscribers of received entire message history
        /// </summary>
        /// <param name="allMessages">The entire message history to call OnAllMessages function of subscribers with</param>
        public void Notify(List<ChatContext> allMessages)
        {
            Trace.WriteLine("[ContentClient] Notifying subscribers of all messages shared in the meeting until now");
            foreach (var subscriber in _subscribers) subscriber.OnAllMessages(allMessages);
        }


        /// <summary>
        ///     Handles received messages from the network
        /// </summary>
        /// <param name="message">The received message</param>
        public void OnReceive(MessageData message)
        {
            _messageHandlers[message.Event](message);
        }

        public void NewMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received new message from server");
            // in case there is any file data associated with the message (there shouldn't be)
            // make file data null
            if (message.FileData != null) message.FileData = null;

            ReceiveMessageData receivedMessage = message;

            // add the message to the correct ChatContext in allMessages
            var key = receivedMessage.ReplyThreadId;
            if (_contextMap.ContainsKey(key))
            {
                var index = _contextMap[key];
                _allMessages[index].MsgList.Add(receivedMessage);
                _allMessages[index].NumOfMessages += 1;
            }
            else // in case the message is part of a new thread
            {
                // create new thread with given id
                var newContext = new ChatContext();
                newContext.ThreadId = key;
                newContext.MsgList.Add(receivedMessage);
                newContext.NumOfMessages = 1;
                newContext.CreationTime = receivedMessage.SentTime;

                _allMessages.Add(newContext);

                // add entry in the hash table to keep track of ChatContext with given thread Id
                var index = _allMessages.Count - 1;
                _contextMap.Add(key, index);
            }

            // notify the subscribers of the new message
            Notify(receivedMessage);
        }

        public void UpdateMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received message update from server");
            if (message.FileData != null) message.FileData = null;

            ReceiveMessageData receivedMessage = message;

            // update the message in _allMessages
            var key = receivedMessage.ReplyThreadId;
            if (_contextMap.ContainsKey(key))
            {
                var index = _contextMap[key];
                var numMessages = _allMessages[index].NumOfMessages;
                int i;
                for (i = 0; i < numMessages; i++)
                {
                    var id = _allMessages[index].MsgList[i].MessageId;
                    if (id == receivedMessage.MessageId)
                    {
                        _allMessages[index].MsgList[i] = receivedMessage;
                        break;
                    }
                }

                // if no match was found, there is an error
                if (i == numMessages) throw new ArgumentException("No message with given id exists");
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
            Trace.WriteLine("[ContentClient] Received message star event from server");
            var contextId = message.ReplyThreadId;
            var messageId = message.MessageId;

            if (_contextMap.ContainsKey(contextId))
            {
                var index = _contextMap[contextId];
                var numMessages = _allMessages[index].NumOfMessages;
                int i;
                for (i = 0; i < numMessages; i++)
                {
                    var id = _allMessages[index].MsgList[i].MessageId;
                    if (id == messageId)
                    {
                        _allMessages[index].MsgList[i].Starred = true;
                        break;
                    }
                }

                // if no match was found, there is an error
                if (i == numMessages) throw new ArgumentException("No message with given id exists");
            }
            else
            {
                throw new ArgumentException("No message thread with given id exists");
            }

            Notify(message);
        }

        public void DownloadMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received requested file from server");
            var savedirpath = message.Message;
            var savepath = savedirpath + message.FileData.fileName;

            Trace.WriteLine("[ContentClient] Saving file to path: {0}", savepath);
            File.WriteAllBytes(savepath, message.FileData.fileContent);
        }
    }
}