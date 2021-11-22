using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content;

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
        private List<ChatContext> chats;
    }
}
