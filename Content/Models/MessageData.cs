using MongoDB.Bson.Serialization.Attributes;

namespace Content
{
    [BsonIgnoreExtraElements]
    public class MessageData : ReceiveMessageData
    {
        /// <summary>
        /// File data such as the content of the file as bytes, its size, etc.
        /// </summary>
        public SendFileData FileData;
    }
}