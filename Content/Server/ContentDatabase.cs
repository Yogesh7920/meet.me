/// <author>Sameer Dhiman</author>
/// <created>18/10/2021</created>
/// <summary>
///     This files handles storing and fecthing files and chats to and from memory
/// </summary>
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

        /// <summary>
        /// Constructor for ContentDatabase, initilizes all the member variables.
        /// </summary>
        public ContentDatabase()
        {
            _files = new Dictionary<int, MessageData>();
            _chatContexts = new List<ChatContext>();
            _chatContextsMap = new Dictionary<int, int>();
            IdGenerator.ResetChatContextId();
            IdGenerator.ResetMessageId();
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
            // If requested messageId is not in the map return null
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
            messageData.MessageId = IdGenerator.GetMessageId();
            // If message is a part of already existing chatContext
            if (_chatContextsMap.ContainsKey(messageData.ReplyThreadId))
            {
                int threadIndex = _chatContextsMap[messageData.ReplyThreadId];
                ChatContext chatContext = _chatContexts[threadIndex];
                ReceiveMessageData msg = messageData.Clone();
                chatContext.AddMessage(msg);
            }
            // else create a new chatContext and add the message to it
            else
            {
                ChatContext chatContext = new ChatContext();
                int newThreadId = IdGenerator.GetChatContextId();
                messageData.ReplyThreadId = newThreadId;
                ReceiveMessageData msg = messageData.Clone();
                chatContext.AddMessage(msg);

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
            // If given ChatContext or Message doesn't exists return null
            if (!_chatContextsMap.ContainsKey(replyThreadId))
            {
                return null;
            }

            int threadIndex = _chatContextsMap[replyThreadId];

            ChatContext chatContext = _chatContexts[threadIndex];

            // If given ChatContext doesn't contain the message return null
            if (!chatContext.ContainsMessageId(messageId))
            {
                return null;
            }

            int messageIndex = chatContext.RetrieveMessageIndex(messageId);

            return chatContext.MsgList[messageIndex];
        }
    }
}