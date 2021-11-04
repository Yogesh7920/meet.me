using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Content
{
    [BsonIgnoreExtraElements]
    public class SendFileData
    {
        public string fileName;

        [BsonId]
        public ObjectId messageId;

        public byte[] fileContent;

        public long fileSize;
    }
}