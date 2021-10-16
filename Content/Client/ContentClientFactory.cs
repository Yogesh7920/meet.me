using System;
namespace Content
{
    public class ContentClientFactory
    {
        private ContentClientFactory() { }
        private static IContentClient _instance;

        public static void setUser(int userId)
        {
            throw new NotImplementedException();
        }
        public static IContentClient getInstance()
        {
            throw new NotImplementedException();
        }
    }
}
