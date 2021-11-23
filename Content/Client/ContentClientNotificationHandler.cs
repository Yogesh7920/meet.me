using System;
using System.Collections.Generic;
using System.Diagnostics;
using Networking;

namespace Content
{
    public class ContentClientNotificationHandler : INotificationHandler
    {
        private readonly ContentClient _contentHandler;

        private readonly ISerializer _serializer;

        public ContentClientNotificationHandler(IContentClient contentHandler)
        {
            _serializer = new Serializer();
            _contentHandler = contentHandler as ContentClient;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            Trace.WriteLine("[ContentClientNotificationHandler] Deserializing data received from network");
            string deserializedType = _serializer.GetObjectType(data, "Content");

            if (string.Equals(deserializedType,"Content.MessageData"))
            {
                MessageData receivedMessage = _serializer.Deserialize<MessageData>(data);
                _contentHandler.OnReceive(receivedMessage);
            }

            else if (string.Equals(deserializedType, "Content.ArrayOfChatContext"))
            {
                List<ChatContext> allMessages = _serializer.Deserialize<List<ChatContext>>(data);
                _contentHandler.Notify(allMessages);
            }

            else
            {
                throw new ArgumentException($"Deserialized object of unknown type: {deserializedType}");
            }
        }
    }
}