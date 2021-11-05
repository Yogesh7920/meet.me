using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace Content
{
    internal class ChatContextServer
    {
        private List<ChatContext> _allMessages;
        private ContentDatabase _contentDatabase;

        public ChatContextServer(ContentDatabase contentDatabase)
        {
            this._allMessages = new List<ChatContext>();
            this._contentDatabase = contentDatabase;
        }

        public void Receive(MessageData messageData)
        {
            UpdateChatContext(messageData);
        }

        public List<ChatContext> GetAllMessages()
        {
            return _allMessages;
        }

        private void UpdateChatContext(ReceiveMessageData receiveMessageData)
        {
            if (receiveMessageData.ReplyThreadId != ObjectId.Empty)
            {
                ChatContext chatContext = _allMessages.FirstOrDefault(chatContext => chatContext.ThreadId == receiveMessageData.ReplyThreadId);
                switch (receiveMessageData.Event)
                {
                    case MessageEvent.NewMessage:
                        chatContext.MsgList.Add(receiveMessageData);
                        chatContext.NumOfMessages++;
                        break;

                    case MessageEvent.Star:
                        StarMessage(receiveMessageData, chatContext);
                        break;

                    case MessageEvent.Update:
                        UpdateMessage(receiveMessageData, chatContext);
                        break;

                    default:
                        throw new System.Exception();
                }

                _contentDatabase.UpdateChatContext(chatContext.ThreadId, chatContext);
            }
            else
            {
                ChatContext chatContext = new ChatContext();
                chatContext.CreationTime = System.DateTime.Now;
                chatContext.NumOfMessages = 1;
                chatContext.MsgList = new List<ReceiveMessageData>();
                chatContext.MsgList.Add(receiveMessageData);
                _contentDatabase.Store(chatContext);
                _allMessages.Add(chatContext);
            }
        }

        private void StarMessage(ReceiveMessageData receiveMessageData, ChatContext chatContext)
        {
            ReceiveMessageData message = chatContext.MsgList.FirstOrDefault(message => message.MessageId == receiveMessageData.MessageId);
            message.Starred = !message.Starred;
        }

        private void UpdateMessage(ReceiveMessageData receiveMessageData, ChatContext chatContext)
        {
            ReceiveMessageData message = chatContext.MsgList.FirstOrDefault(message => message.MessageId == receiveMessageData.MessageId);
            message.Message = receiveMessageData.Message;
        }
    }
}