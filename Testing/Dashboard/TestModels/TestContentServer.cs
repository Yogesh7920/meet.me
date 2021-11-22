using Content;
using System.Collections.Generic;

namespace Testing.Dashboard.TestModels
{
    class TestContentServer : IContentServer
    {
        public TestContentServer()
        {
            gotNotificationToSendMessages = false;
            chats = new();
        }

        public void Reset()
        {
            gotNotificationToSendMessages = false;
            chats = new();
        }

        public List<ChatContext> SGetAllMessages()
        {
            return chats;
        }

        public void SSendAllMessagesToClient(int userId)
        {
            gotNotificationToSendMessages = true;
        }

        public void SSubscribe(IContentListener subscriber)
        {

        }

        public bool gotNotificationToSendMessages;
        public List<ChatContext> chats;
    }
}
