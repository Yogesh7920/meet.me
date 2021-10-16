using System;

namespace Content
{
    public class ReceiveMessageData
    {
        public MessageEvent messageEvent; // will be one of update, new message or star
        public MessageType messageType; // enum/string indicating file or chat
        public string message; // message if message type is chat / filename and size if file
        public int messageId;
        public int senderId;
        public int[] receiverIds; // list of receiver ids
        public int replyThreadId; // special value (-1) if message not part of any existing thread otherwise thread id of thread to which the message belongs
        public DateTime sentTime; // time at which message was sent
        public bool starred; // value indicating whether a message is to be included in the summary
    }
}