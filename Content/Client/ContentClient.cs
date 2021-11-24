/// <author>Yuvraj Raghuvanshi</author>
/// <created>16/10/2021</created>
using Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Content
{
    internal class ContentClient : IContentClient
    {
        private int _userId;

        private List<ChatContext> _allMessages;
        // a map from thread (thread id) to index of that thread in _allMessages
        private Dictionary<int, int> _contextMap;

        private ICommunicator _communicator;

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

        // getters and setters
        public ICommunicator Communicator
        {
            get => _communicator;
            set
            {
                _communicator = value;
                _communicator.Subscribe("Content", _notifHandler);
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

                // set _allMessages
                _allMessages = allMessages;

                // rebuild mapping
                int len = _allMessages.Count;
                for (int i = 0; i < len; i++)
                {
                    // key
                    int threadId = _allMessages[i].ThreadId;
                    // value is the index, which is the loop index i
                    // add key value pair to dictionary
                    _contextMap.Add(threadId, i);
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

            // if the message is part of a thread, ensure thread exists
            if (toSend.ReplyThreadId != -1)
                if (!_contextMap.ContainsKey(toSend.ReplyThreadId))
                    throw new ArgumentException($"Thread with given thread id ({toSend.ReplyThreadId}) doesn't exist");

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
            var found = 0;
            foreach (var chatThread in _allMessages)
            {
                foreach (var msg in chatThread.MsgList)
                    if (msg.MessageId == messageId)
                    {
                        found = 1;
                        if (msg.Type != MessageType.File)
                            throw new ArgumentException("Invalid message ID: Message requested for download is not a file type message");
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
            ReceiveMessageData msg = RetrieveMessage(messageId);

            if (msg is null)
            {
                throw new ArgumentException($"Message with given message id doesn't exist");
            }

            if (msg.Type == MessageType.Chat)
                _chatHandler.ChatStar(messageId);
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
                    _chatHandler.ChatUpdate(messageId, newMessage);
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

            // add the message to the correct ChatContext in allMessages
            var key = receivedMessage.ReplyThreadId;

            if (key == -1)
                throw new ArgumentException("Reply thread id of received message cannot be -1");

            // use locks because the list of chat contexts may be shared across multiple threads
            lock (_lock)
            {
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
            }
            // notify the subscribers of the new message
            Notify(receivedMessage);
        }

        private void UpdateMessageHandler(MessageData message)
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
                // again, use locks to ensure thread-safe updation
                lock (_lock)
                {
                    for (i = 0; i < numMessages; i++)
                    {
                        var id = _allMessages[index].MsgList[i].MessageId;
                        if (id == receivedMessage.MessageId)
                        {
                            _allMessages[index].MsgList[i] = receivedMessage;
                            break;
                        }
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

        private void StarMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received message star event from server");
            var contextId = message.ReplyThreadId;
            var messageId = message.MessageId;

            if (_contextMap.ContainsKey(contextId))
            {
                var index = _contextMap[contextId];
                var numMessages = _allMessages[index].NumOfMessages;
                int i;
                lock (_lock)
                {
                    for (i = 0; i < numMessages; i++)
                    {
                        var id = _allMessages[index].MsgList[i].MessageId;
                        if (id == messageId)
                        {
                            bool starStatus = _allMessages[index].MsgList[i].Starred;
                            _allMessages[index].MsgList[i].Starred = !starStatus;
                            break;
                        }
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

        private void DownloadMessageHandler(MessageData message)
        {
            Trace.WriteLine("[ContentClient] Received requested file from server");
            var savedirpath = message.Message;
            var savepath = savedirpath + message.FileData.fileName;

            Trace.WriteLine("[ContentClient] Saving file to path: {0}", savepath);
            File.WriteAllBytes(savepath, message.FileData.fileContent);
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
            foreach (var context in _allMessages)
            {
                foreach (var message in context.MsgList)
                {
                    if (message.MessageId == messageid)
                        return message;
                }
            }

            return null;
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
            }
        }
    }
}