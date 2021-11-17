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

        public ContentClientNotificationHandler()
        {
            _serializer = new Serializer();
            _contentHandler = ContentClientFactory.getInstance() as ContentClient;
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            Trace.WriteLine("[ContentClientNotificationHandler] Deserializing data received from network");
            var deserialized = _serializer.Deserialize<object>(data);

            if (deserialized is MessageData)
            {
                var receivedMessage = deserialized as MessageData;
                _contentHandler.OnReceive(receivedMessage);
            }

            else if (deserialized is List<ChatContext>)
            {
                var allMessages = deserialized as List<ChatContext>;
                _contentHandler.Notify(allMessages);
            }

            else
            {
                throw new ArgumentException("Deserialized object of unknown type");
            }
        }

        /// <inheritdoc />
        public void OnClientJoined<T>(T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}