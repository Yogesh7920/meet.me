using System;

namespace Content
{
    public class ReceiveMessageData
    {
        /// <summary>
        /// Message Event - Update, NewMessage, Star, Download
        /// </summary>
        public MessageEvent messageEvent;

        /// <summary>
        /// Message Type - File or Chat
        /// </summary>
        public MessageType messageType;

        /// <summary>
        /// Message string if messageType is Chat else file name for File messageType
        /// </summary>
        public string message;

        /// <summary>
        /// Id of the message
        /// </summary>
        public int messageId;

        /// <summary>
        /// User id of the message sender
        /// </summary>
        public int senderId;

        /// <summary>
        /// List of ids for receviers, all if empty
        /// </summary>
        public int[] receiverIds;

        /// <summary>
        /// Id of thread to which this message belongs
        /// </summary>
        public int replyThreadId;

        /// <summary>
        /// Time at which message was sent
        /// </summary>
        public DateTime sentTime;

        /// <summary>
        /// True if this message is starred else False
        /// </summary>
        public bool starred;
    }
}