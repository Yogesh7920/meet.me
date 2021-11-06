using Networking;
using System;
using System.Collections.Generic;

namespace Content
{
    public class ContentClientNotificationHandler : INotificationHandler
    {

        private ISerializer _serializer;
        private ContentClient _contentHandler;
        
        public ContentClientNotificationHandler()
        {
            _serializer = new Serializer();
            _contentHandler = ContentClientFactory.getInstance() as ContentClient;
        }

        /// <inheritdoc/>
        public void OnDataReceived(string data)
        {
            Object deserialized = _serializer.Deserialize<Object>(data);

            if (deserialized is MessageData)
            {
                MessageData receivedMessage = deserialized as MessageData;
                _contentHandler.OnReceive(receivedMessage);
            }

            else if (deserialized is List<ChatContext>)
            {
                List<ChatContext> allMessages = deserialized as List<ChatContext>;
                _contentHandler.Notify(allMessages);
            }
            
            else
                throw new ArgumentException("Deserialized object of unknown type");
        }

        /// <inheritdoc/>
        public void OnClientJoined<T>(T socketObject)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}