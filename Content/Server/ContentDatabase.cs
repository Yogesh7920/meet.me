using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Testing")]

namespace Content
{
    internal class ContentDatabase
    {
        private Dictionary<int, MessageData> _messages;
        private Dictionary<int, ChatContext> _chatContexts;

        public ContentDatabase()
        {
            _messages = new Dictionary<int, MessageData>();
        }

        public MessageData Store(MessageData messageData)
        {
            messageData.MessageId = IdGenerator.getMessageId();
            _messages[messageData.MessageId] = messageData;
            return messageData;
        }

        public void Store(ChatContext chatContext)
        {
            chatContext.ThreadId = IdGenerator.getChatContextId();
            _chatContexts[chatContext.ThreadId] = chatContext;
        }

        public void UpdateMessageData(int id, MessageData messageData)
        {
            _messages[id] = messageData;
        }

        public void UpdateChatContext(int id, ChatContext chatContext)
        {
            _chatContexts[id] = chatContext;
        }

        public MessageData RetrieveMessage(int messageId)
        {
            return _messages[messageId];
        }
    }
}