using System;
using System.Collections.Generic;

namespace Content
{
    public class ChatContext
    {
        /// <summary>
        /// List of all the messages in the thread
        /// </summary>
        public List<ReceiveMessageData> MsgList;

        /// <summary>
        /// Id of the thread
        /// </summary>
        public int ThreadId;

        /// <summary>
        /// Number of messages in the thread
        /// </summary>
        public int NumOfMessages;

        /// <summary>
        /// Time of creation of thread
        /// </summary>
        public DateTime CreationTime;
    }
}
