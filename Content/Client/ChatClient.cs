using System;
using Networking;

namespace Content
{
    internal class ChatClient
    {
        private readonly ICommunicator _communicator;
        private readonly string _moduleIdentifier = "Content";
        private readonly ISerializer _serializer;

        public ChatClient()
        {
            _communicator = CommunicationFactory.GetCommunicator();
            _serializer = new Serializer();
        }

        public int UserId { get; set; }

        private MessageData SendToMessage(SendMessageData toconvert, MessageEvent ChatEvent)
        {
            var Converted = new MessageData();
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
            var tosend = SendToMessage(toserver, MessageEvent.NewMessage);
            tosend.MessageId = -1;
            var xml = _serializer.Serialize(tosend);
            _communicator.Send(xml, _moduleIdentifier);
        }

        public string ChatNewMessage(SendMessageData toserver, bool testFlag)
        {
            var tosend = SendToMessage(toserver, MessageEvent.NewMessage);
            tosend.MessageId = -1;
            var xml = _serializer.Serialize(tosend);
            return xml;
        }

        public void ChatUpdate()
        {
            throw new NotImplementedException();
        }

        public void ChatStar(int messageId)
        {
            var toSend = new MessageData();
            toSend.MessageId = messageId;
            toSend.Event = MessageEvent.Star;
            toSend.SenderId = UserId;

            var xml = _serializer.Serialize(toSend);
            _communicator.Send(xml, _moduleIdentifier);
        }

        public string ChatStar(int messageId, bool testFlag)
        {
            var toSend = new MessageData();
            toSend.MessageId = messageId;
            toSend.Event = MessageEvent.Star;
            toSend.SenderId = UserId;

            var xml = _serializer.Serialize(toSend);
            return xml;
        }
    }
}