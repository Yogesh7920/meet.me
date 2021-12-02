/// <author>Yuvraj Raghuvanshi</author>
/// <created>16/10/2021</created>

using System.Diagnostics;

namespace Content
{
    public class ContentClientFactory
    {
        private static ContentClient instance;
        private static readonly object _lock = new();

        private ContentClientFactory()
        {
        }

        private static void CreateInstance()
        {
            Trace.WriteLine("[ContentClientFactory] IContentClient instance created");
            // acquire lock before creating object to ensure thread saftey
            lock (_lock)
            {
                // check to ensure a thread acquiring lock just after
                // creation by another thread doesn't create a new object
                if (instance == null) instance = new ContentClient();
            }
        }

        /// <summary>
        ///     Sets user ID of instance
        ///     if the instance doesn't exist yet, it is created
        /// </summary>
        /// <param name="userId">User ID of the client</param>
        public static void SetUser(int userId)
        {
            if (instance == null) CreateInstance();
            instance.UserId = userId;
            instance.RequestMessageHistory();
        }

        /// <summary>
        ///     Returns singleton instance of an implementation of the
        ///     IContentClient interface in a thread safe way
        /// </summary>
        /// <returns>Object that implements the IContentClient interface</returns>
        public static IContentClient GetInstance()
        {
            if (instance == null) CreateInstance();
            return instance;
        }
    }
}