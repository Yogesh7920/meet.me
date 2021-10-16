using System;
using System.Collections.Generic;

namespace Content
{
    public class Thread
    {
        public List<ReceiveMessageData> msgList;
        public int threadId;
        public int numOfMessages;
        public DateTime creationTime;
    }
}