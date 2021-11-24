using System;
using System.Collections.Generic;

namespace Content
{
    public class ChatContext
    {
        /// <summary>
        ///     Time of creation of thread
        /// </summary>
        public DateTime CreationTime;

        /// <summary>
        ///     List of all the messages in the thread
        /// </summary>
        public List<ReceiveMessageData> MsgList;

        /// <summary>
        ///     Number of messages in the thread
        /// </summary>
        public int NumOfMessages;

        /// <summary>
        ///     Id of the thread
        /// </summary>
        public int ThreadId;

        // dictionary mapping message id to its index in MsgList
        private Dictionary<int, int> messageIds;

        public ChatContext()
        {
            CreationTime = new DateTime();
            MsgList = new List<ReceiveMessageData>();
            NumOfMessages = 0;
            ThreadId = -1;
            messageIds = new Dictionary<int, int>();
        }

        public void AddMessage(ReceiveMessageData msg)
        {
            if (!MessageIsValid(msg.Message))
                throw new ArgumentException("Invalid message string");

            if (ThreadId == -1) // new thread without any messages in it
            {
                if (msg.ReplyThreadId == -1)
                    throw new ArgumentException("Thread id of a message can't be -1");

                MsgList.Add(msg);
                NumOfMessages++;
                ThreadId = msg.ReplyThreadId;
                CreationTime = msg.SentTime;
                messageIds.Add(msg.MessageId, NumOfMessages - 1);
            }

            else
            {
                if (msg.ReplyThreadId != ThreadId)
                    throw new ArgumentException("Invalid thread id, message doesn't belong in this thread");

                if (messageIds.ContainsKey(msg.MessageId))
                    throw new ArgumentException("Message with given message id already exists in thread");

                MsgList.Add(msg);
                NumOfMessages++;
                messageIds.Add(msg.MessageId, NumOfMessages - 1);

            }
        }

        public void UpdateMessage(int messageId, string newMessage)
        {

            if (!messageIds.ContainsKey(messageId))
                throw new ArgumentException("Message with given message id doesn't exist in thread");

            if (!MessageIsValid(newMessage))
                throw new ArgumentException("Invalid message string");

            int index = messageIds[messageId];
            if (MsgList[index].Type != MessageType.Chat)
                throw new ArgumentException("Message requested for update is not chat");
            
            MsgList[index].Message = newMessage;
        }

        public int RetrieveMessageIndex(int messageId)
        {
            if (!messageIds.ContainsKey(messageId))
                throw new ArgumentException("Message with given message id doesn't exist in thread");

            return messageIds[messageId];
        }

        private bool MessageIsValid(string message)
        {
            if (message == null || message == "")
                return false;
            else
                return true;
        }
    }
}