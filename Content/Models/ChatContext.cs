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
    }
}