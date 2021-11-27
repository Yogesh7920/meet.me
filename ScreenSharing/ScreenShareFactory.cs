/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 25/11/2021
 * date modified: 25/11/2021
**/

namespace ScreenSharing
{
    /// <summary>
    /// Factory for the ScreenShareClient object.
    /// </summary>
    public static class ScreenShareFactory
    {
        // Singleton ScreenShareClient instance.
        private static ScreenShareClient ScreenSharer;

        /// <summary>
        /// Static constructor for the factory.
        /// </summary>
        static ScreenShareFactory()
        {
            ScreenSharer = new ScreenShareClient();
        }

        /// <summary>
        /// Gets an instance of the ScreenShareClient.
        /// </summary>
        /// <returns>ScreenShareClient instance.</returns>
        public static ScreenShareClient GetScreenSharer()
        {
            return ScreenSharer;
        }
    }
}
