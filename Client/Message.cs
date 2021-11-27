using System;

namespace Client
{
    public class Message
    {
        /// <summary>
        ///     Message string if MessageType is Chat else file path if MessageType is File
        /// </summary>
        public int MessageId { get; set; }
        public string UserName { get; set; }
        public bool Type { get; set; } //true-msg
        public string TextMessage { get; set; }
        public string Time { get; set; }
        public string ReplyMessage { get; set; }
        public bool ToFrom { get; set; } //true-sent

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
