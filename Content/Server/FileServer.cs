using System.Diagnostics;

namespace Content
{
    internal class FileServer
    {
        private readonly ContentDatabase _contentDatabase;

        public FileServer(ContentDatabase contentDatabase)
        {
            _contentDatabase = contentDatabase;
        }

        /// <summary>
        /// Recevies the request related to files, will store the new file or return the file requested
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new file message without file data or the file requested</returns>
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
                    return FetchFile(messageData);

                default:
                    Trace.WriteLine($"[FileServer] Unkown Event {messageData.Event} for file type");
                    return null;
            }
        }

        /// <summary>
        /// Saves file in the contentDatabase.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the saved file message without the file data.</returns>
        private MessageData SaveFile(MessageData messageData)
        {
            messageData = _contentDatabase.StoreFile(messageData).Clone();
            // the object is going to be typecasted to ReceiveMessageData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            messageData.FileData = null;
            return messageData;
        }

        /// <summary>
        /// Fetches a stored file.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the requested file message.</returns>
        private MessageData FetchFile(MessageData messageData)
        {
            MessageData receiveMessageData = _contentDatabase.GetFiles(messageData.MessageId);

            if (receiveMessageData == null)
            {
                Trace.WriteLine($"[FileServer] File not found messageId: {messageData.MessageId}.");
                return null;
            }

            MessageData downloadMessageData = receiveMessageData.Clone();

            // store file path on which the file will be downloaded on the client's system
            downloadMessageData.Message = messageData.Message;
            downloadMessageData.Event = MessageEvent.Download;
            downloadMessageData.SenderId = messageData.SenderId;
            return downloadMessageData;
        }
    }
}