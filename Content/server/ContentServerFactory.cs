namespace Content
{
    public class ContentServerFactory
    {
        private static IContentServer contentServer;

        /// <summary>
        /// Singleton factory for ContentServer
        /// </summary>
        /// <returns>
        /// Return instance of ContentServer
        /// </returns>
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