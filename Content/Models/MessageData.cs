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

        /// <summary>
        /// Clones the messageData object
        /// </summary>
        /// <returns>Returns the cloned instance of the object</returns>

        public MessageData()
        {
        }

        public MessageData Clone()
        {
            return (MessageData)MemberwiseClone();
        }
    }
}