using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Content
{
    [BsonIgnoreExtraElements]
    public class ReceiveMessageData
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
        /// Message string if messageType is Chat else file name for File messageType
        /// </summary>
        public string Message;

        /// <summary>
        /// Id of the message
        /// </summary>
        [BsonId]
        public ObjectId MessageId;

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