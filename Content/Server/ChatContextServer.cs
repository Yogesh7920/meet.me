using System.Collections.Generic;
using System.Diagnostics;

namespace Content
{
    internal class ChatContextServer
    {
        private ContentDatabase _contentDatabase;

        public ChatContextServer(ContentDatabase contentDatabase)
        {
            this._contentDatabase = contentDatabase;
        }

        public ReceiveMessageData Receive(MessageData messageData)
        {
            Trace.WriteLine("[ContentServer] Received message from ContentServer");
            return UpdateChatContext(messageData);
        }

        public List<ChatContext> GetAllMessages()
        {
            return _contentDatabase.GetChatContexts();
        }

        private ReceiveMessageData UpdateChatContext(MessageData receiveMessageData)
        {
            switch (receiveMessageData.Event)
            {
                case MessageEvent.NewMessage:
                    Trace.WriteLine("[ChatContextServer] Event is NewMessage, Adding message to existing Thread");
                    return _contentDatabase.StoreMessage(receiveMessageData);

                case MessageEvent.Star:
                    Trace.WriteLine("[ChatContextServer] Event is Star, Starring message in existing Thread");
                    return StarMessage(receiveMessageData);

                case MessageEvent.Update:
                    Trace.WriteLine("[ChatContextServer] Event is Update, Updating message in existing Thread");
                    return UpdateMessage(receiveMessageData);

                default:
                    Debug.Assert(false, "[ChatContextServer] Unkown Event");
                    return null;
            }
        }

        private ReceiveMessageData StarMessage(MessageData receiveMessageData)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(receiveMessageData);
            message.Starred = !message.Starred;
            _contentDatabase.UpdateMessage(message);
            return message;
        }

        private ReceiveMessageData UpdateMessage(MessageData receiveMessageData)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(receiveMessageData);
            message.Message = receiveMessageData.Message;
            _contentDatabase.UpdateMessage(message);
            return message;
        }
    }
}