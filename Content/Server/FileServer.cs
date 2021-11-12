using System;
using System.Diagnostics;

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
            Trace.WriteLine("[FileServer] Received message from ContentServer");
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    Trace.WriteLine("[FileServer] MessageEvent is NewMessage, Storing this file");
                    return SaveFile(messageData);

                case MessageEvent.Download:
                    Trace.WriteLine("[FileServer] MessageEvent is Download, Fetching the file");
                    return FetchFile(messageData.MessageId);

                default:
                    Debug.Assert(false, "[File Server] Unknown Event");
                    return null;
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

        private MessageData FetchFile(int id)
        {
            return _contentDatabase.RetrieveMessage(id);
        }
    }
}