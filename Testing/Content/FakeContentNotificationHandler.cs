/// <author>Vishesh Munjal</author>
/// <created>24/11/2021</created>
/// <summary>
/// This file mimics the function of a ContentHandler in the Content in order to test its functions
/// </summary>
using Content;
using System;
using System.Collections.Generic;
using Networking;

namespace Testing.Content
{
    public class FakeNotifier: ContentClientNotificationHandler
    {
        private MessageData _msgData;
        private List<ChatContext> _listChatContext;
        private ISerializer _serializer;
        public FakeNotifier(IContentClient contentClient) : base(contentClient)
        {
            _msgData = new MessageData();
            _listChatContext = new List<ChatContext>();
            _serializer = new Serializer();
        }

        private void Reset()
        {
            _msgData = new MessageData();
            _listChatContext = new List<ChatContext>();
        }

        public MessageData GetMessageData()
        {
            Reset();
            _msgData = base._receivedMessage;
            return _msgData;
        }

        public List<ChatContext> GetAllMessageData()
        {
            Reset();
            _listChatContext = base._allMessages;
            return _listChatContext;
        }
    }
}