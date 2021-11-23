using System;
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
        public MessageData Receive(MessageData messageData)
        {
            ReceiveMessageData receiveMessageData;
            Trace.WriteLine("[ContentServer] Received message from ContentServer");
            switch (messageData.Event)
            {
                case MessageEvent.NewMessage:
                    Trace.WriteLine("[ChatContextServer] Event is NewMessage, Adding message to existing Thread");
                    return _contentDatabase.StoreMessage(messageData);

                case MessageEvent.Star:
                    Trace.WriteLine("[ChatContextServer] Event is Star, Starring message in existing Thread");
                    receiveMessageData = StarMessage(messageData.ReplyThreadId, messageData.MessageId);
                    break;

                case MessageEvent.Update:
                    Trace.WriteLine("[ChatContextServer] Event is Update, Updating message in existing Thread");
                    receiveMessageData = UpdateMessage(messageData.ReplyThreadId, messageData.MessageId, messageData.Message);
                    break;

                default:
                    Trace.WriteLine($"Uknown Event {messageData.Event} for chat type.");
                    return null;
            }

            if (receiveMessageData == null)
            {
                return null;
            }

            return new MessageData(receiveMessageData);
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
        private ReceiveMessageData StarMessage(int replyThreadId, int messageId)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(replyThreadId, messageId);

            if (message == null)
            {
                Trace.WriteLine($"[ChatContextServer] Message not found replyThreadID: {replyThreadId}, messageId: {messageId}.");
                return null;
            }

            message.Starred = !message.Starred;
            message.Event = MessageEvent.Star;
            return message;
        }

        /// <summary>
        /// Updates the message
        /// </summary>
        /// <param name="receiveMessageData"></param>
        /// <returns>Returns the message after updating it</returns>
        private ReceiveMessageData UpdateMessage(int replyThreadId, int messageId, string msgString)
        {
            ReceiveMessageData message = _contentDatabase.GetMessage(replyThreadId, messageId);

            if (message == null)
            {
                Trace.WriteLine($"[ChatContextServer] Message not found replyThreadID: {replyThreadId}, messageId: {messageId}.");
                return null;
            }

            message.Message = msgString;
            message.Event = MessageEvent.Update;
            return message;
        }
    }
}