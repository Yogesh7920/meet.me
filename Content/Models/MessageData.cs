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
        ///     Contructor for MessageData from ReceiveMessageData
        /// </summary>
        /// <param name="msgData"></param>
        /// <returns></returns>
        public MessageData(ReceiveMessageData msgData)
        {
            Event = msgData.Event;
            Message = msgData.Message;
            MessageId = msgData.MessageId;
            ReceiverIds = msgData.ReceiverIds;
            SenderId = msgData.SenderId;
            ReplyThreadId = msgData.ReplyThreadId;
            ReplyMsgId = msgData.ReplyMsgId;
            Starred = msgData.Starred;
            Type = msgData.Type;
            SentTime = msgData.SentTime;
        }

        /// <summary>
        ///     Clones the messageData object
        /// </summary>
        /// <returns>Returns the cloned instance of the object</returns>
        public MessageData Clone()
        {
            return (MessageData) MemberwiseClone();
        }
    }
}