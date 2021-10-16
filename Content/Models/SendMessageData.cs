namespace Content
{
    public class SendMessageData
    {
        public MessageType messageType; // enum indicating file or chat
        public string message; // message if chat, filepath if message type is File
        public int receiverIds[]; // list of receiver ids
        public int replyThreadId; // special value (-1) if message not part of any existing thread 
                           // otherwise thread id of thread to which the message belongs
    }
}