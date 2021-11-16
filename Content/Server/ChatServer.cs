using System;
using System.Diagnostics;

namespace Content
{
    internal class ChatServer
    {
        private ContentDatabase _contentDatabase;

        public ChatServer(ContentDatabase contentDatabase)
        {
            this._contentDatabase = contentDatabase;
        }

        public MessageData Receive(MessageData messageData)
        {
            Trace.WriteLine("[ChatServer] Received message from ContentServer");
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    Trace.WriteLine("[ChatServer] MessageEvent is NewMessage, Storing this message");
                    return Store(messageData);

                case MessageEvent.Star:
                    Trace.WriteLine("[ChatServer] MessageEvent is Star, Starring this message");
                    return Star(messageData.MessageId);

                case MessageEvent.Update:
                    Trace.WriteLine("[ChatServer] MessageEvent is Update, Updating this message");
                    return Update(messageData.MessageId, messageData);

                default:
                    Debug.Assert(false, "[ChatServer] Unknown Event");
                    return null;
            }
        }

        private MessageData Update(int id, MessageData messageData)
        {
            _contentDatabase.UpdateMessageData(id, messageData);
            return messageData;
        }

        private MessageData Star(int id)
        {
            MessageData messageData = Fetch(id);
            messageData.Starred = !messageData.Starred;
            _contentDatabase.UpdateMessageData(id, messageData);
            return messageData;
        }

        private MessageData Store(MessageData messageData)
        {
            return _contentDatabase.Store(messageData);
        }

        private MessageData Fetch(int messageId)
        {
            return _contentDatabase.RetrieveMessage(messageId);
        }
    }
}