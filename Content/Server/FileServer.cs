using MongoDB.Bson;
using System;

namespace Content
{
    internal class FileServer
    {
        private ContentDatabase _contentDatabase;

        public FileServer(ContentDatabase contentDatabase)
        {
            this._contentDatabase = contentDatabase;
        }

        public MessageData Receive(MessageData messageData)
        {
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    return SaveFile(messageData);

                case MessageEvent.Download:
                    return FetchFile(messageData.MessageId);

                default:
                    throw new SystemException();
            }
        }

        private MessageData SaveFile(MessageData messageData)
        {
            messageData = _contentDatabase.Store(messageData);
            // the object is going to be typecasted to ReceiveMessageData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            messageData.FileData = null;
            return messageData;
        }

        private MessageData FetchFile(ObjectId id)
        {
            return _contentDatabase.RetrieveMessage(id);
        }
    }
}