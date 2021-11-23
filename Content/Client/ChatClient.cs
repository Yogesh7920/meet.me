/// <author>Vishesh Munjal</author>
/// <created>1/11/2021</created>
using System;
using Networking;
using System.Collections.Generic;
using System.Diagnostics;

namespace Content
{
    internal class ChatClient
    {
        private ICommunicator _communicator;
        public ICommunicator Communicator
        {
            get => _communicator;
            set => _communicator = value;
        }

        private readonly string _moduleIdentifier = "Content";
        private readonly ISerializer _serializer;

        public ChatClient(ICommunicator communicator)
        {
            //_communicator = CommunicationFactory.GetCommunicator();
            _communicator = communicator;
            _serializer = new Serializer();
        }

        public int UserId { get; set; }

        public MessageData SendToMessage(SendMessageData toconvert, MessageEvent ChatEvent)
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
			Trace.WriteLine("[ChatClient Converting SendMessageData object to a MessageData object");
            return Converted;
        }

        public void ChatNewMessage(SendMessageData toserver)
        {
            var tosend = SendToMessage(toserver, MessageEvent.NewMessage);
            tosend.MessageId = -1;
            var xml = _serializer.Serialize(tosend);
			Trace.WriteLine("[ChatClient] Marking Event of chat as NewMessage and sending to server");
            _communicator.Send(xml, _moduleIdentifier);
        }

        public void ChatUpdate(int messageId, string newMessage)
        {
			
			var toSend = new MessageData();
			toSend.MessageId = messageId;
            toSend.Event = MessageEvent.Update;
            toSend.SenderId = UserId;
			toSend.Message = newMessage;
			toSend.Type = MessageType.Chat;
			var xml = _serializer.Serialize(toSend);
			Trace.WriteLine("[ChatClient] Marking Event of chat as update and sending to server");
            _communicator.Send(xml, _moduleIdentifier);
        }

        public void ChatStar(int messageId)
        {
            var toSend = new MessageData();
            toSend.MessageId = messageId;
            toSend.Event = MessageEvent.Star;
            toSend.SenderId = UserId;
			toSend.Type = MessageType.Chat;
            var xml = _serializer.Serialize(toSend);
			 Trace.WriteLine("[ChatClient] Marking Event of chat as star and sending to server");
            _communicator.Send(xml, _moduleIdentifier);
        }
    }
}