namespace Content
{
    public interface IContentClient
    {
        void CSend(SendMessageData toSend);
        void CDownload(int messageId, string savePath);
        void CMarkStar(int messageId);
        void CUpdateChat(int messageId, string newMessage);
        void CSubscribe(IContentListener subscriber);
        Thread CGetThread(int threadId);
    }
}