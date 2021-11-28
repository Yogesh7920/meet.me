/// <author>Yuvraj Raghuvanshi</author>
/// <created>16/10/2021</created>
using Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private int _userId;

        private List<ChatContext> _allMessages;
        // a map from thread (thread id) to index of that thread in _allMessages
        private Dictionary<int, int> _contextMap;
        // a map from message ids to their thread id
        private Dictionary<int, int> _messageIdMap;

        private ICommunicator _communicator;
        private ISerializer _serializer;

        private object _lock;
        private ChatClient _chatHandler;
        private FileClient _fileHandler;

        // dictionary that maps message events to their respective handler functions
        private Dictionary<MessageEvent, Action<MessageData>> _messageHandlers;
        private INotificationHandler _notifHandler;

        private List<IContentListener> _subscribers;

        public ContentClient()
        {
            _userId = -1;
            _lock = new();
            _subscribers = new List<IContentListener>();
            _allMessages = new List<ChatContext>();
            _contextMap = new Dictionary<int, int>();
            _messageIdMap = new Dictionary<int, int>();

            // add message handler functions for each event
            _messageHandlers = new Dictionary<MessageEvent, Action<MessageData>>();
            _messageHandlers[MessageEvent.NewMessage] = NewMessageHandler;
            _messageHandlers[MessageEvent.Update] = UpdateMessageHandler;
            _messageHandlers[MessageEvent.Star] = StarMessageHandler;
            _messageHandlers[MessageEvent.Download] = DownloadMessageHandler;

            // subscribe to the network
            _notifHandler = new ContentClientNotificationHandler(this);
            _communicator = CommunicationFactory.GetCommunicator();
            _serializer = new Serializer();
            try
            {
                _communicator.Subscribe("Content", _notifHandler);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClient] Exception encountered during subscribing to networking module: {e.GetType().Name}: {e.Message}");
            }

            // initialize file handler and chat handler
            _fileHandler = new FileClient(_communicator);
            _chatHandler = new ChatClient(_communicator);
        }

        // getters and setters
        public ICommunicator Communicator
        {
            get => _communicator;
            set
            {
                _communicator = value;
                try
                {
                    _communicator.Subscribe("Content", _notifHandler);
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"[ContentClient] Exception encountered when using networking module: {e.GetType().Name}: {e.Message}");
                }
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

        public List<ChatContext> AllMessages
        {
            get => _allMessages;
        }

        private void setAllMessages(List<ChatContext> allMessages)
        {
            lock (_lock)
            {
                // clear context map
                _contextMap = new Dictionary<int, int>();
                // clear message id map
                _messageIdMap = new Dictionary<int, int>();

                // set _allMessages
                _allMessages = allMessages;

                // rebuild mappings
                int len = _allMessages.Count;
                for (int i = 0; i < len; i++)
                {
                    ChatContext context = _allMessages[i];
                    // mapping for _contextMap
                    int threadId = context.ThreadId;
                    // value is the index, which is the loop index i
                    // add key value pair to dictionary
                    _contextMap.Add(threadId, i);

                    // now for the mapping of messages to thread id, i.e _messageIdMap
                    foreach(ReceiveMessageData msg in context.MsgList)
                    {
                        // key is message id and value is thread id
                        _messageIdMap.Add(msg.MessageId, threadId);
                    }
                }
            }
        }


        // interface functions

        /// <inheritdoc/>
        public int GetUserId()
        {
            return _userId;
        }

        /// <inheritdoc />
        public void CSend(SendMessageData toSend)
        {
            // ensure receiver ids isn't null
            if (toSend.ReceiverIds is null)
                throw new ArgumentException("List of receiver ids given is null");

            // if the message is part of a thread
            if (toSend.ReplyThreadId != -1)
            {
                //  ensure thread exists
                if (!_contextMap.ContainsKey(toSend.ReplyThreadId))
                    throw new ArgumentException($"Thread with given reply thread id ({toSend.ReplyThreadId}) doesn't exist");
            }

            // if the message is a reply
            if (toSend.ReplyMsgId != -1)
            {
                // validate the reply message id
                ValidateSentReplyMsgId(toSend.ReplyMsgId, toSend.ReplyThreadId);

                // modify the receiver ids to the intersection of given recipients and the precursor message's recipients
                // this way, the privacy level of the precursor message is preserved
                ReceiveMessageData precursorMessage = RetrieveMessage(toSend.ReplyMsgId);
                if (precursorMessage is null)
                    throw new ArgumentException("Invalid reply message id: Message being replied to doesn't exist");
                // sender id of the precursor also has to be counted in the intersection
                toSend.ReceiverIds = ReceiverIntersection(precursorMessage.ReceiverIds, precursorMessage.SenderId, toSend.ReceiverIds);
            }
                
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
            if (!IsPathWritable(savePath))
                throw new ArgumentException("Given file path is not writable");

            // check that the message with ID messageID exists and is indeed a file
            ReceiveMessageData msg = RetrieveMessage(messageId);
            
            if (msg is null)
            {
                Trace.WriteLine("[ContentClient] File requested for download not found");
                throw new ArgumentException("Message with given message ID not found");
            }

            if (msg.Type != MessageType.File)
                throw new ArgumentException("Invalid message ID: Message requested for download is not a file type message");

            _fileHandler.Download(messageId, savePath);
        }

        /// <inheritdoc />
        public void CMarkStar(int messageId)
        {
            ReceiveMessageData msg = RetrieveMessage(messageId);

            if (msg is null)
            {
                throw new ArgumentException($"Message with given message id doesn't exist");
            }          

            if (msg.Type == MessageType.Chat)
                _chatHandler.ChatStar(messageId, msg.ReplyThreadId);
            else
                throw new ArgumentException($"Message with given message id is not chat");
        }

        /// <inheritdoc />
        public void CUpdateChat(int messageId, string newMessage)
        {
            ReceiveMessageData msg = RetrieveMessage(messageId);

            if (msg is null)
            {
                throw new ArgumentException($"Message with given message id doesn't exist");
            }

            if (msg.Type == MessageType.Chat)
            {
                if (msg.SenderId == UserId)
                    _chatHandler.ChatUpdate(messageId, msg.ReplyThreadId, newMessage);
                else
                    throw new ArgumentException("Update not allowed for messages from another sender");
            }

            else
                throw new ArgumentException($"Message type is not chat");
        }

        /// <inheritdoc />
        public void CSubscribe(IContentListener subscriber)
        {
            Trace.WriteLine("[ContentClient] Added new subscriber");
            if (subscriber is null)
                throw new ArgumentException("Subscriber is null");

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

        // handler functions

        /// <summary>
        ///     Handles received messages from the network
        /// </summary>
        /// <param name="message">The received message</param>
        public void OnReceive(MessageData message)
        {
            if (message is null)
            {
                throw new ArgumentException("Received null message");
            }
            _messageHandlers[message.Event](message);
        }

        public void OnReceive(List<ChatContext> allMessages)
        {
            if (allMessages is null)
                throw new ArgumentException("Null message in argument");

            Trace.WriteLine("[ContentClient] Received message history from server");
            // since the received message history is from the server and thus more definitive,
            // replace the current message history with it
            setAllMessages(allMessages);
            Notify(allMessages);
        }

        private void NewMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received new message from server");
            // in case there is any file data associated with the message (there shouldn't be)
            // make file data null
            if (message.FileData != null) message.FileData = null;
            ReceiveMessageData receivedMessage = message;

            // check if message id isn't a duplicate
            if (_messageIdMap.ContainsKey(receivedMessage.MessageId))
                throw new ArgumentException("New message has duplicate message id which matches a previous message");

            var key = receivedMessage.ReplyThreadId;

            if (key == -1)
                throw new ArgumentException("Reply thread id of received message cannot be -1");

            // if the message is a reply, ensure that the message being replied to exists
            if (message.ReplyMsgId != -1)
            {
                ValidateReceivedReplyMsgId(receivedMessage.ReplyMsgId, receivedMessage.ReplyThreadId);
            }

            // add the message to the correct ChatContext in allMessages
            // use locks because the list of chat contexts may be shared across multiple threads
            lock (_lock)
            {

                // add message id to the set of message ids
                _messageIdMap.Add(receivedMessage.MessageId, key);

                if (_contextMap.ContainsKey(key))
                {
                    var index = _contextMap[key];
                    _allMessages[index].AddMessage(receivedMessage);
                }
                else // in case the message is part of a new thread
                {
                    // create new thread with given id
                    var newContext = new ChatContext();
                    newContext.AddMessage(receivedMessage);
                    _allMessages.Add(newContext);

                    // add entry in the hash table to keep track of ChatContext with given thread Id
                    var index = _allMessages.Count - 1;
                    _contextMap.Add(key, index);
                }
            }
            // notify the subscribers of the new message
            Notify(receivedMessage);
        }

        private void UpdateMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received message update from server");
            if (message.FileData != null) message.FileData = null;

            ReceiveMessageData receivedMessage = message;
            int messageId = receivedMessage.MessageId;

            // make sure message being updated exists
            if (!_messageIdMap.ContainsKey(messageId))
                throw new ArgumentException("Failed to update because message with given message id doesn't exist");

            // update the message in _allMessages
            var key = _messageIdMap[messageId];
            if (_contextMap.ContainsKey(key))
            {
                var index = _contextMap[key];
                // again, use locks to ensure thread-safe updation
                lock (_lock)
                {
                    string newMessage = receivedMessage.Message;
                    _allMessages[index].UpdateMessage(messageId, newMessage);
                }
            }
            else
            {
                throw new ArgumentException("No message thread with given id exists");
            }

            // notify subscribers
            Notify(receivedMessage);
        }

        private void StarMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received message star event from server");
            var messageId = message.MessageId;

            if (!_messageIdMap.ContainsKey(messageId))
                throw new ArgumentException("Message with given message id doesn't exist");

            var contextId = _messageIdMap[messageId];

            if (_contextMap.ContainsKey(contextId))
            {
                var index = _contextMap[contextId];
                lock (_lock)
                {
                    _allMessages[index].StarMessage(messageId);
                }
            }
            else
            {
                throw new ArgumentException("No message thread with given id exists");
            }

            Notify(message);
        }

        private void DownloadMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received requested file from server");
            var savepath = message.Message;

            Trace.WriteLine($"[ContentClient] Saving file to path: {savepath}");
            File.WriteAllBytes(savepath, message.FileData.fileContent);
        }

        // utility functions not part of the interface

        /// <summary>
        ///     Sends a message to the server to send all messages received on the server until now
        /// </summary>
        public void RequestMessageHistory()
        {
            // the only fields that matter are type and sender id
            MessageData msg = new MessageData();
            msg.SenderId = UserId;
            msg.Type = MessageType.HistoryRequest;

            try
            {
                // serialize the message and send via network
                var toSendSerialized = _serializer.Serialize(msg);
                Trace.WriteLine($"[ContentClient] Sending request for message history to server for user id {UserId}");
                _communicator.Send(toSendSerialized, "Content");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClient] Exception encountered during sending message history request: {e.GetType().Name}: {e.Message}");
            }
        }

        // helper methods

        /// <summary>
        ///     Notify all subscribers of received message
        /// </summary>
        /// <param name="message">The message object to call OnMessage function of subscribers with</param>
        private void Notify(ReceiveMessageData message)
        {
            if (message is null)
                throw new ArgumentException("Null message in argument");

            Trace.WriteLine("[ContentClient] Notifying subscribers of new received message");
            foreach (var subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnMessage(message); });
            }
        }

        /// <summary>
        ///     Notify all subscribers of received entire message history
        /// </summary>
        /// <param name="allMessages">The entire message history to call OnAllMessages function of subscribers with</param>
        private void Notify(List<ChatContext> allMessages)
        {
            if (allMessages is null)
                throw new ArgumentException("Null message in argument");

            Trace.WriteLine("[ContentClient] Notifying subscribers of all messages shared in the meeting until now");
            foreach (var subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnAllMessages(allMessages); });
            }
        }

        /// <summary>
        /// Helper function that retrieves a message from inbox using message id
        /// </summary>
        /// <param name="messageid">Id of the message to be retrieved</param>
        /// <returns>ReceiveMessageData object if message exists otherwise null</returns>
        private ReceiveMessageData RetrieveMessage(int messageid)
        {
            if (!_messageIdMap.ContainsKey(messageid))
                return null;

            int threadId = _messageIdMap[messageid];
            int contextIndex = _contextMap[threadId];
            ChatContext context = _allMessages[contextIndex];

            int msgIndex = context.RetrieveMessageIndex(messageid);

            return context.MsgList[msgIndex];
        }

        /// <summary>
        /// Checks if a filepath is writable
        /// </summary>
        /// <param name="path">path to check</param>
        /// <returns>true if path is writable, false otherwise</returns>
        private bool IsPathWritable(string path)
        {
            try
            {
                using (FileStream fs = File.Create(path, 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // reset function (useful for testing singletons)
        // resets members that have some state
        public void Reset()
        {
            _userId = -1;
            _subscribers = new List<IContentListener>();

            lock (_lock)
            {
                _allMessages = new List<ChatContext>();
                _contextMap = new Dictionary<int, int>();
                _messageIdMap = new Dictionary<int, int>();
            }
        }

        private void ValidateSentReplyMsgId(int replyMsgId, int threadId)
        {
            // ensure the message being replied to exists
            if (!_messageIdMap.ContainsKey(replyMsgId))
                throw new ArgumentException("Invalid reply message id: Message being replied to doesn't exist");

            if (threadId != -1)
            {
                // check equality with the thread id of the message being replied to
                if (_messageIdMap[replyMsgId] != threadId)
                    throw new ArgumentException("Invalid reply message id and thread id combination: Message being replied to is part of a different thread than the reply");
            }
        }

        private void ValidateReceivedReplyMsgId(int replyMsgId, int threadId)
        {
            if (!_messageIdMap.ContainsKey(replyMsgId))
                throw new ArgumentException("Invalid reply message id: Message being replied to doesn't exist");

            // if the received message is part of a new thread, no check required
            if (!_contextMap.ContainsKey(threadId))
                return;

            // otherwise check equality with the thread id of the message being replied to
            if (_messageIdMap[replyMsgId] != threadId)
                throw new ArgumentException("Invalid reply message id and thread id combination: Message being replied to is part of a different thread than the reply");
        }

        private int[] ReceiverIntersection(int[] precursorReceiverIds, int precursorSenderId, int[] replyReceiverIds)
        {
            // special case for empty array, which means broadcast, so the intersection is just the other array
            if (precursorReceiverIds.Length == 0)
                return replyReceiverIds;

            // the sender should also be counted as one of the receivers of the precursor, so append it to the precursor receiver ids
            precursorReceiverIds = precursorReceiverIds.Concat(new int[] { precursorSenderId }).ToArray();
            if (replyReceiverIds.Length == 0)
                return precursorReceiverIds; ; 

            // take intersection
            int[] intersection = precursorReceiverIds.Intersect(replyReceiverIds).ToArray();
            // if intersection is empty, it means that the list of recievers of the first message and the reply are disjoint
            // which means the reply can't be sent to anyone, in which case we raise an error
            if (intersection.Length > 0)
                return intersection;
            else
                throw new ArgumentException("Invalid list of receivers, the reply can't be sent to anyone because of the privacy of the precursor message");
        }
    }
}