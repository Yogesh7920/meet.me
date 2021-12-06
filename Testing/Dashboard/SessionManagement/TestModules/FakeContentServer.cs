/// <author> Rajeev Goyal </author>
/// <created> 24/112021 </created>
/// <summary>
/// This file contains the fake content server which provides the session manager with chats.
/// </summary>

using System;
using System.Collections.Generic;
using Content;

namespace Testing.Dashboard.SessionManagement.TestModules
{
    public class FakeContentServer : IContentServer
    {
        public List<ChatContext> allChats;

        public FakeContentServer()
        {
            allChats = new List<ChatContext>();
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
    }
}