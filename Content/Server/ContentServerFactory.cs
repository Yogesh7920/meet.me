/// <author>Sameer Dhiman</author>
/// <created>18/10/2021</created>
/// <summary>
///     This file is a singleton factory for ContentServer
/// </summary>

namespace Content
{
    public class ContentServerFactory
    {
        private static IContentServer _contentServer;

        /// <summary>
        ///     Singleton factory for ContentServer
        /// </summary>
        /// <returns>
        ///     Return instance of ContentServer
        /// </returns>
        public static IContentServer GetInstance()
        {
            if (_contentServer != null) return _contentServer;

            _contentServer = new ContentServer();
            return _contentServer;
        }
    }
}