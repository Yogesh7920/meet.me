/// <author>Suchitra Yechuri</author>
/// <created>18/10/2021</created>
/// <summary>
///     This file contains the message data type for storing the message details.
/// </summary>

using System;

namespace Client
{
    public class Message
    {
        /// <summary>
        ///     Message Id of the message
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        ///     Name of the user who sent the message
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Type of the message
        ///     True if the message type is a chat 
        ///     False if the message type is a file
        /// </summary>
        public bool Type { get; set; }

        /// <summary>
        ///     Message string if message type is chat else file name for file message type
        /// </summary>
        public string TextMessage { get; set; }

        /// <summary>
        ///     Time at which message was sent
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        ///     Message string of the message being replied to
        ///     Null if it's not a reply message
        /// </summary>
        public string ReplyMessage { get; set; }

        /// <summary>
        ///     True, if the message was sent by the current user, else false
        /// </summary>
        public bool ToFrom { get; set; }

        public Message()
        {
            MessageId = -1;
            UserName = null;
            Type = true;
            TextMessage = null;
            Time = DateTime.Now.ToShortTimeString();
            ReplyMessage = null;
            ToFrom = false;
        }
    }
}
