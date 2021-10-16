namespace Content
{
    public class ContentServerFactory
    {
        private static IContentServer contentServer;

        public static IContentServer GetInstance()
        {
            if (contentServer != null)
            {
                return contentServer;
            }

            contentServer = new ContentServer();
            return contentServer;
        }
    }
}