using System;
using System.Collections.Generic;

namespace Content
{
    internal class ChatClient
    {
        private int UserId;
        private string module_Indentifier = "Content";

        private MessageData SendToMessage(SendMessageData toconvert, MessageEvent ChatEvent)
        {
            MessageData Converted = new MessageData();
            Converted.Event = ChatEvent;
            Converted.Type = toconvert.Type;
            Converted.Message = toconvert.Message;
            Converted.FileData = null;
            Converted.SenderId = UserId;
            Converted.ReceiverIds = toconvert.ReceiverIds;
            Converted.ReplyThreadId = toconvert.ReplyThreadId;
            Converted.Starred = false;
            Converted.SentTime = DateTime.Now;
            return Converted;
        }
        public void ChatNewMessage(SendMessageData toserver)
        {
            MessageData tosend = SendToMessage(toserver,MessageEvent.NewMessage);
            tosend.MessageId = -1;
            string xml = Serializer.Serialize(tosend);
            communicator.Send(xml,module_Indentifier);

        }

        public void ChatUpdate()
        {
            throw new NotImplementedException();
        }

        public void ChatStar(SendMessageData toserver, int messageId)
        {
            MessageData tosend = SendToMessage(toserver, ChatEvent);
            tosend.Starred = true;
            tosend.MessageId = messageId;
            string xml = Serializer.Serialize(tosend);
            communicator.Send(xml,module_Indentifier);
            
        }

        public void OnReceive()
        {
            throw new NotImplementedException();
        }

    }

}