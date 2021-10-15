using System;
using System.Collections.Generic;
namespace Content
{
    class Thread
    {
        List<ReceiveMessageData> msgList;
        int threadId;
        int numOfMessages;
        DateTime creationTime;
    }
}