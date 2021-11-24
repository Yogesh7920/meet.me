/// <author>Vishesh Munjal</author>
/// <created>1/11/2021</created>
/// <summary>
/// This file is the ChatClient of the Sending part of the Client to Content server and handles chat operation
/// such as newMessage, Update, and star etc.
/// </summary>
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
		/// <summary>
		/// This simply checks the validity of the message.
		/// </summary>
		private bool MessageIsvalid(string message)
		{
			if(message == null || message == "")
				return false;

			return true;
		}

        public int UserId { get; set; }
		/// <summary>
		/// This function is the conversion from the SendMessageData to the MessageData object 
		/// </summary>
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
		/// <summary>
		/// This function gets the SendMessageData from the ContentClient and calls the SendToMessage function
		/// to convert to MessageData, then sets the event, Serialize and sends to Content server via Networking.
		/// </summary>
        public void ChatNewMessage(SendMessageData toserver)
        {
			if(MessageIsvalid(toserver.Message)){
				var tosend = SendToMessage(toserver, MessageEvent.NewMessage);
				tosend.MessageId = -1;
				var xml = _serializer.Serialize(tosend);
				Trace.WriteLine("[ChatClient] Marking Event of chat as NewMessage and sending to server");
				_communicator.Send(xml, _moduleIdentifier);
			}
			else
			{
				throw new ArgumentException("Invalid Message String");
			}
        }
		/// <summary>
		/// This function gets the messageId and new message to change.
		/// Then creates a MessageData, then sets the event, Serialize, change the message and sends to Content server via Networking.
		/// </summary>
        public void ChatUpdate(int messageId, string newMessage)
        {
			if(MessageIsvalid(newMessage)){
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
			else
			{
				throw new ArgumentException("Invalid Message String");
			}

			
        }
		/// <summary>
		/// This function simply takes a message Id and send it in the form of a MessageData to the Content Server 
		/// where in the message is marked Star.
		/// </summary>
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