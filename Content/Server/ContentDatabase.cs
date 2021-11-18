using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Testing")]

namespace Content
{
    internal class ContentDatabase
    {
        private Dictionary<int, MessageData> _files;
        private List<ChatContext> _chatContexts;
        private Dictionary<int, int> _chatContextsMap;
        private Dictionary<int, int> _messageMap;

        public ContentDatabase()
        {
            _files = new Dictionary<int, MessageData>();
            _chatContexts = new List<ChatContext>();
            _chatContextsMap = new Dictionary<int, int>();
            _messageMap = new Dictionary<int, int>();
            IdGenerator.resetChatContextId();
            IdGenerator.resetMessageId();
        }

        public MessageData StoreFile(MessageData messageData)
        {
            MessageData message = StoreMessage(messageData);
            _files[message.MessageId] = messageData;
            return message;
        }

        public MessageData GetFiles(int messageId)
        {
            return _files[messageId];
        }

        public MessageData StoreMessage(MessageData messageData)
        {
            messageData.MessageId = IdGenerator.getMessageId();
            if (_chatContextsMap.ContainsKey(messageData.ReplyThreadId))
            {
                int threadIndex = _chatContextsMap[messageData.ReplyThreadId];
                ChatContext chatContext = _chatContexts[threadIndex];
                chatContext.MsgList.Add(messageData);
                chatContext.NumOfMessages++;
                _messageMap[messageData.MessageId] = chatContext.NumOfMessages - 1;
            }
            else
            {
                ChatContext chatContext = new ChatContext();
                chatContext.CreationTime = messageData.SentTime;
                chatContext.NumOfMessages = 1;
                chatContext.MsgList = new List<ReceiveMessageData>();
                chatContext.ThreadId = IdGenerator.getChatContextId();
                messageData.ReplyThreadId = chatContext.ThreadId;
                chatContext.MsgList.Add(messageData);

                _messageMap[messageData.MessageId] = 0;
                _chatContexts.Add(chatContext);
                _chatContextsMap[chatContext.ThreadId] = _chatContexts.Count - 1;
            }

            return messageData;
        }

        public List<ChatContext> GetChatContexts()
        {
            return _chatContexts;
        }

        public ReceiveMessageData GetMessage(int replyThreadId, int messageId)
        {
            int threadIndex = _chatContextsMap[replyThreadId];
            int messageIndex = _messageMap[messageId];
            return _chatContexts[threadIndex].MsgList[messageIndex];
        }
    }
}