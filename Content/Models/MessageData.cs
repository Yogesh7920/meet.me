using System;

namespace Content
{
    public class MessageData
    {
        /// <summary>
        /// Message Event - Update, NewMessage, Star, Download
        /// </summary>
        public MessageEvent Event;

        /// <summary>
        /// Message Type - File or Chat
        /// </summary>
        public MessageType Type;

        /// <summary>
        /// Message string if messageType is Chat else file name
        /// </summary>
        public string Message;

        /// <summary>
        /// Id of the message
        /// </summary>
        public int MessageId;

        /// <summary>
        /// File data such as the content of the file as bytes, its size, etc.
        /// </summary>
        public SendFileData FileData;

        /// <summary>
        /// User id of the message sender
        /// </summary>
        public int SenderId;

        /// <summary>
        /// List of ids for receviers, all if empty
        /// </summary>
        public int[] ReceiverIds;

        /// <summary>
        /// Id of thread to which this message belongs
        /// </summary>
        public int ReplyThreadId;

        /// <summary>
        /// Time at which message was sent
        /// </summary>
        public DateTime SentTime;

        /// <summary>
        /// True if this message is starred else False
        /// </summary>
        public bool Starred;
    }
}