using System;

namespace Content
{
    [Serializable]
    public class MessageData : ReceiveMessageData
    {
        /// <summary>
        ///     File data such as the content of the file as bytes, its size, etc.
        /// </summary>
        public SendFileData FileData;

        public MessageData Clone()
        {
            return (MessageData)MemberwiseClone();
        }
    }
}