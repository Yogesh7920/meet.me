namespace Content
{
    public interface IContentListener
    {
        void OnMessage(ReceiveMessageData messageData);
        void OnAllMessages(List<Thread> allMessages);
    }
}