/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake content server which provides the session manager with chats.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeContentServer : IContentServer
    {
        public FakeContentServer()
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
