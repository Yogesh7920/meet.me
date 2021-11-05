using MongoDB.Bson.Serialization.Attributes;
using System;
using System.IO;

namespace Content
{
    [BsonIgnoreExtraElements]
    [Serializable()]
    public class SendFileData
    {
        public SendFileData(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("File {0} not found", filepath);
            }

            fileName = Path.GetFileName(filepath);
            fileContent = File.ReadAllBytes(filepath);
            fileSize = fileContent.Length;
        }

        public string fileName;

        public byte[] fileContent;

        public long fileSize;
    }
}