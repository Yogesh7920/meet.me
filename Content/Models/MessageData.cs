using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Content
{
    [BsonIgnoreExtraElements]
    [Serializable()]
    public class MessageData : ReceiveMessageData
    {
        /// <summary>
        /// File data such as the content of the file as bytes, its size, etc.
        /// </summary>
        public SendFileData FileData;
    }
}