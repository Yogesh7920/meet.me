using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Networking;

namespace Content
{
    internal class ChatClient
    {
        public int UserId;
        private string module_Indentifier = "Content";
        private ICommunicator _communicator;
        private ISerializer _serializer;

        public ChatClient()
        {
             _communicator = CommunicationFactory.GetCommunicator();
             _serializer = new Serializer();
        }
        private MessageData SendToMessage(SendMessageData toconvert, MessageEvent ChatEvent)
        {
            MessageData Converted = new MessageData();
            Converted.Event = ChatEvent;
            Converted.Type = toconvert.Type;
            Converted.Message = toconvert.Message;
            Converted.FileData = null;
            Converted.SenderId = UserId;
            Converted.ReceiverIds = toconvert.ReceiverIds;
            Converted.ReplyThreadId = new ObjectId(toconvert.ReplyThreadId.ToString());
            Converted.Starred = false;
            Converted.SentTime = DateTime.Now;
            return Converted;
        }
        public void ChatNewMessage(SendMessageData toserver)
        {
            MessageData tosend = SendToMessage(toserver,MessageEvent.NewMessage);
            tosend.MessageId = new ObjectId("-1");
            string xml = _serializer.Serialize<MessageData>(tosend);
            _communicator.Send(xml,module_Indentifier);

        }

        public void ChatUpdate()
        {
            throw new NotImplementedException();
        }

        public void ChatStar(SendMessageData toserver, int messageId)
        {
            MessageData tosend = SendToMessage(toserver, MessageEvent.Star);
            tosend.Starred = true;
            tosend.MessageId = new ObjectId(messageId.ToString());
            string xml = _serializer.Serialize<MessageData>(tosend);
            _communicator.Send(xml,module_Indentifier);
            
        }

       

    }

}