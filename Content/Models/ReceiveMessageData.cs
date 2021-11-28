using System;

namespace Content
{
    [Serializable]
    public class ReceiveMessageData
    {
        /// <summary>
        ///     Message Event - Update, NewMessage, Star, Download
        /// </summary>
        public MessageEvent Event;

        /// <summary>
        ///     Message string if messageType is Chat else file name for File messageType
        /// </summary>
        public string Message;

        /// <summary>
        ///     Id of the message
        /// </summary>
        public int MessageId;

        /// <summary>
        ///     List of ids for receviers, all if empty
        /// </summary>
        public int[] ReceiverIds;

        /// <summary>
        ///     Id of thread to which this message belongs
        /// </summary>
        public int ReplyThreadId;

        /// <summary>
        ///     User id of the message sender
        /// </summary>
        public int SenderId;

        /// <summary>
        ///     Time at which message was sent
        /// </summary>
        public DateTime SentTime;

        /// <summary>
        ///     True if this message is starred else False
        /// </summary>
        public bool Starred;

        /// <summary>
        ///     Message Type - File or Chat
        /// </summary>
        public MessageType Type;

        /// <summary>
        ///     Id of the message being replied to
        /// </summary>
        public int ReplyMsgId;

        public ReceiveMessageData()
        {
            MessageId = -1;
            ReceiverIds = null;
            ReplyThreadId = -1;
            SenderId = -1;
            SentTime = new DateTime();
            Starred = false;
            ReplyMsgId = -1;
        }
    }
}