/// <author>Siddharth Sha</author>
/// <created>15/11/2021</created>
/// <summary>
///		This file contains the test content
///		for testing purpose
/// </summary>

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
