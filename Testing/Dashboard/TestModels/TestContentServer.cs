/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the test content
///		for testing purpose
/// </summary>

using System.Collections.Generic;
using Content;

namespace Testing.Dashboard.TestModels
{
    internal class TestContentServer : IContentServer
    {
        public List<ChatContext> chats;

        public bool gotNotificationToSendMessages;

        public TestContentServer()
        {
            gotNotificationToSendMessages = false;
            chats = new List<ChatContext>();
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

        public void Reset()
        {
            gotNotificationToSendMessages = false;
            chats = new List<ChatContext>();
        }
    }
}