using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Content
{
    internal class ChatContextServer
    {
        private List<ChatContext> _allMessages;
        private ContentDatabase _contentDatabase;

        public ChatContextServer(ContentDatabase contentDatabase)
        {
            this._contentDatabase = contentDatabase;
            this._allMessages = new List<ChatContext>();
        }

        public void Receive(MessageData messageData)
        {
            Trace.WriteLine("[ContentServer] Received message from ContentServer");
            UpdateChatContext(messageData);
        }

        public List<ChatContext> GetAllMessages()
        {
            return _allMessages;
        }

        private void UpdateChatContext(ReceiveMessageData receiveMessageData)
        {
            if (receiveMessageData.ReplyThreadId != -1)
            {
                ChatContext chatContext = _allMessages.FirstOrDefault(chatContext => chatContext.ThreadId == receiveMessageData.ReplyThreadId);
                switch (receiveMessageData.Event)
                {
                    case MessageEvent.NewMessage:
                        Trace.WriteLine("[ChatContextServer] Event is NewMessage, Adding message to existing Thread");
                        chatContext.MsgList.Add(receiveMessageData);
                        chatContext.NumOfMessages++;
                        break;

                    case MessageEvent.Star:
                        Trace.WriteLine("[ChatContextServer] Event is Star, Starring message in existing Thread");
                        StarMessage(receiveMessageData, chatContext);
                        break;

                    case MessageEvent.Update:
                        Trace.WriteLine("[ChatContextServer] Event is Update, Updating message in existing Thread");
                        UpdateMessage(receiveMessageData, chatContext);
                        break;

                    default:
                        Debug.Assert(false, "[ChatContextServer] Unkown Event");
                        return;
                }

                _contentDatabase.UpdateChatContext(chatContext.ThreadId, chatContext);
            }
            else
            {
                Trace.WriteLine("[ChatContextServer] Creating a new ChatContext and adding message to it");
                ChatContext chatContext = new ChatContext();
                chatContext.CreationTime = receiveMessageData.SentTime;
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