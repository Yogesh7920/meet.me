using System;

namespace Content
{
    public class ReceiveMessageData
    {
        public MessageEvent messageEvent;
        public MessageType messageType;
        public string message;
        public int messageId;
        public int senderId;
        public int[] receiverIds;
        public int replyThreadId;
        public DateTime sentTime;
        public bool starred;
    }
}