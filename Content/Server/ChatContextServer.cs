using System.Collections.Generic;
using System.Diagnostics;

namespace Content
{
    internal class ChatContextServer
    {
        private readonly ContentDatabase _contentDatabase;

        /// <summary>
        /// Constructor for ChatContextServer, initializes the contentDatabse
        /// </summary>
        /// <param name="contentDatabase"></param>
        public ChatContextServer(ContentDatabase contentDatabase)
        {
            _contentDatabase = contentDatabase;
        }

        /// <summary>
        /// Handles the chat messages received based on the event
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new message</returns>
        public ReceiveMessageData Receive(MessageData messageData)
        {
            Trace.WriteLine("[ContentServer] Received message from ContentServer");
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    Trace.WriteLine("[ChatContextServer] Event is NewMessage, Adding message to existing Thread");
                    return _contentDatabase.StoreMessage(messageData);

                case MessageEvent.Star:
                    Trace.WriteLine("[ChatContextServer] Event is Star, Starring message in existing Thread");
                    return StarMessage(messageData);

                case MessageEvent.Update:
                    Trace.WriteLine("[ChatContextServer] Event is Update, Updating message in existing Thread");
                    return UpdateMessage(messageData);

                default:
                    Debug.Assert(false, "[ChatContextServer] Unkown Event");
                    return null;
            }
        }

        /// <summary>
        /// Gets all the messages on the server
        /// </summary>
        /// <returns>Returns List of all Chat Contexts</returns>
        public List<ChatContext> GetAllMessages()
        {
            return _contentDatabase.GetChatContexts();
        }

        /// <summary>
        /// Stars a message
        /// </summary>
        /// <param name="receiveMessageData"></param>
        /// <returns>Returns the message after starring it</returns>
        private ReceiveMessageData StarMessage(MessageData receiveMessageData)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(receiveMessageData.ReplyThreadId, receiveMessageData.MessageId);
            message.Starred = !message.Starred;
            message.Event = receiveMessageData.Event;
            return message;
        }

        /// <summary>
        /// Updates the message
        /// </summary>
        /// <param name="receiveMessageData"></param>
        /// <returns>Returns the message after updating it</returns>
        private ReceiveMessageData UpdateMessage(MessageData receiveMessageData)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(receiveMessageData.ReplyThreadId, receiveMessageData.MessageId);
            message.Message = receiveMessageData.Message;
            message.Event = receiveMessageData.Event;
            return message;
        }
    }
}