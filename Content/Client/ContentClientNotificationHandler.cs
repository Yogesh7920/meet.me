/// <author>Vishesh Munjal</author>
/// <created>1/11/2021</created>
/// <summary>
/// This file is the Notification Handler of the Receive part of the Client from Content server. 
/// </summary>

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
        protected List<ChatContext> _allMessages;
        protected MessageData _receivedMessage;

        public ContentClientNotificationHandler(IContentClient contentHandler)
        {
            _serializer = new Serializer();
            _contentHandler = contentHandler as ContentClient;
        }

        /// <summary>
        ///     The function is called in order to find out firstly if we are getting a valid message object Type in form
        ///     of the serialized string. Then according to that particular object we further called the required function
        ///     of ContentClient after Deserializing.
        /// </summary>
        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            Trace.WriteLine("[ContentClientNotificationHandler] Deserializing data received from network");

            try
            {
                var deserializedType = _serializer.GetObjectType(data, "Content");

                if (string.Equals(deserializedType, typeof(MessageData).ToString()))
                {
                    _receivedMessage = _serializer.Deserialize<MessageData>(data);
                    _contentHandler.OnReceive(_receivedMessage);
                }

                else if (string.Equals(deserializedType, typeof(List<ChatContext>).ToString()))
                {
                    _allMessages = _serializer.Deserialize<List<ChatContext>>(data);
                    _contentHandler.OnReceive(_allMessages);
                }

                else
                {
                    throw new ArgumentException($"Deserialized object of unknown type: {deserializedType}");
                }
            }
            catch (ArgumentException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    $"[ContentClientNotificationHandler] Exception occurred during deserialization of received data: {e.GetType().Name}: {e.Message}");
            }
        }
    }
}