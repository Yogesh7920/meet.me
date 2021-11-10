namespace Client
{
    public class Message
    {
        public int MessageId { get; set; }
        public string UserName { get; set; }
        public bool Type { get; set; } //true-msg
        public string TextMessage { get; set; }
        public string Time { get; set; }
        public string ReplyMessage { get; set; }
        public bool ToFrom { get; set; } //true-sent
    }
}
