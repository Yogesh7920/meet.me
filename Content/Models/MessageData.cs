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

        public MessageData()
        {
        }

        /// <summary>
        /// Clones the messageData object
        /// </summary>
        /// <returns>Returns the cloned instance of the object</returns>
        public MessageData Clone()
        {
            return (MessageData)MemberwiseClone();
        }

        /// <summary>
        /// Contructor for MessageData from ReceiveMessageData
        /// </summary>
        /// <param name="msgData"></param>
        /// <returns></returns>
        public MessageData(ReceiveMessageData msgData)
        {
            this.Event = msgData.Event;
            this.Message = msgData.Message;
            this.MessageId = msgData.MessageId;
            this.ReceiverIds = msgData.ReceiverIds;
            this.SenderId = msgData.SenderId;
            this.ReplyThreadId = msgData.ReplyThreadId;
            this.Starred = msgData.Starred;
            this.Type = msgData.Type;
        }
    }
}