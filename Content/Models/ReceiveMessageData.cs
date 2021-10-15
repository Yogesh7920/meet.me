using System;
namespace Content
{
    class ReceiveMessageData
    {
        MessageEvent messageEvent; // will be one of update, new message or star
        MessageType messageType; // enum/string indicating file or chat
        string message; // message if message type is chat / filename and size if file
        int messageId;
        int senderId;
        int[] receiverIds; // list of receiver ids
        int replyThreadId; // special value (-1) if message not part of any existing thread otherwise thread id of thread to which the message belongs
        DateTime sentTime; // time at which message was sent
        bool starred; // value indicating whether a message is to be included in the summary
    }
}