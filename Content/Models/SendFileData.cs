using System;
using System.IO;

namespace Content
{
    [Serializable]
    public class SendFileData
    {
        public byte[] fileContent;

        public string fileName;

        public long fileSize;

        public SendFileData()
        {
            fileContent = null;
            fileName = "";
            fileSize = 0;
        }

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
    }
}