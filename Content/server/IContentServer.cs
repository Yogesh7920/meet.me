using System.Collections.Generic;

namespace Content
{
    public interface IContentServer
    {
        void SSubscribe(IContentListener subscriber);

        List<Thread> SGetAllMessages();

        void SSendAllMessagesToClient(int userId);
    }
}