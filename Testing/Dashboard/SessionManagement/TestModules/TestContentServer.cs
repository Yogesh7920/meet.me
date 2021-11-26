using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class TestContentServer : IContentServer
    {
        public TestContentServer()
        {
            allChats = new();
        }

        public List<ChatContext> SGetAllMessages()
        {
            return allChats;
        }

        public void SSendAllMessagesToClient(int userId)
        {
            
        }

        public void SSubscribe(IContentListener subscriber)
        {
            throw new NotImplementedException();
        }

        public List<ChatContext> allChats;
    }
}
