using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Testing")]

namespace Content
{
    internal class ContentDatabase
    {
        private readonly Dictionary<int, MessageData> _files;
        private readonly List<ChatContext> _chatContexts;
        private readonly Dictionary<int, int> _chatContextsMap;
        private readonly Dictionary<int, int> _messageMap;

        /// <summary>
        /// Constructor for ContentDatabase, initilizes all the memeber variables.
        /// </summary>
        public ContentDatabase()
        {
            _files = new Dictionary<int, MessageData>();
            _chatContexts = new List<ChatContext>();
            _chatContextsMap = new Dictionary<int, int>();
            _messageMap = new Dictionary<int, int>();
            IdGenerator.resetChatContextId();
            IdGenerator.resetMessageId();
        }

        /// <summary>
        /// Stores Files in a map
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the data that was stored</returns>
        public MessageData StoreFile(MessageData messageData)
        {
            MessageData message = StoreMessage(messageData);
            _files[message.MessageId] = messageData;
            return message;
        }

        /// <summary>
        /// Fectches the files from a map based on the messageId.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns>Returns the stored file</returns>
        public MessageData GetFiles(int messageId)
        {
            if (!_files.ContainsKey(messageId))
            {
                return null;
            }
            return _files[messageId];
        }

        /// <summary>
        /// Stores a message in _chatContexts, if a message is part of already existing thread appends the message to it
        /// else creates a new thread and appends the message to the new thread.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Retuns the new message stored</returns>
        public MessageData StoreMessage(MessageData messageData)
        {
            messageData.MessageId = IdGenerator.getMessageId();
            if (_chatContextsMap.ContainsKey(messageData.ReplyThreadId))
            {
                int threadIndex = _chatContextsMap[messageData.ReplyThreadId];
                ChatContext chatContext = _chatContexts[threadIndex];
                ReceiveMessageData msg = new ReceiveMessageData(messageData);
                chatContext.MsgList.Add(msg);
                chatContext.NumOfMessages++;
                _messageMap[messageData.MessageId] = chatContext.NumOfMessages - 1;
            }
            else
            {
                ChatContext chatContext = new ChatContext
                {
                    CreationTime = messageData.SentTime,
                    NumOfMessages = 1,
                    MsgList = new List<ReceiveMessageData>(),
                    ThreadId = IdGenerator.getChatContextId()
                };
                messageData.ReplyThreadId = chatContext.ThreadId;
                ReceiveMessageData msg = new ReceiveMessageData(messageData);
                chatContext.MsgList.Add(msg);

                _messageMap[messageData.MessageId] = 0;
                _chatContexts.Add(chatContext);
                _chatContextsMap[chatContext.ThreadId] = _chatContexts.Count - 1;
            }
            return messageData;
        }

        /// <summary>
        /// Gets all the Chat Contexts
        /// </summary>
        /// <returns>Returns all the ChatContexts</returns>
        public List<ChatContext> GetChatContexts()
        {
            return _chatContexts;
        }

        /// <summary>
        /// Gets a particular message based on its messageId and replyThreadId
        /// </summary>
        /// <param name="replyThreadId"></param>
        /// <param name="messageId"></param>
        /// <returns>Returns the requested message</returns>
        public ReceiveMessageData GetMessage(int replyThreadId, int messageId)
        {
            if (!(_chatContextsMap.ContainsKey(replyThreadId) && _messageMap.ContainsKey(messageId)))
            {
                return null;
            }
            int threadIndex = _chatContextsMap[replyThreadId];
            int messageIndex = _messageMap[messageId];

            if (_chatContexts[threadIndex].MsgList.Count < messageIndex + 1)
            {
                return null;
            }

            return _chatContexts[threadIndex].MsgList[messageIndex];
        }
    }
}