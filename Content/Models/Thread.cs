using System;
using System.Collections.Generic;

namespace Content
{
    public class Thread
    {
        /// <summary>
        /// List of all the messages in the thread
        /// </summary>
        public List<ReceiveMessageData> msgList;

        /// <summary>
        /// Id of the thread
        /// </summary>
        public int threadId;

        /// <summary>
        /// Number of messages in the thread
        /// </summary>
        public int numOfMessages;

        /// <summary>
        /// Time of creation of thread
        /// </summary>
        public DateTime creationTime;
    }
}