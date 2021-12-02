/**
 * owned by: Neeraj Patil
 * created by: Neeraj Patil
 * date created: 25/11/2021
 * date modified: 27/11/2021
**/

using System;
using Networking;

namespace ScreenSharing
{
    /// <summary>
    ///     Factory for the ScreenShareClient object.
    /// </summary>
    public static class ScreenShareFactory
    {
        private static readonly Lazy<ScreenShareClient> s_clientScreenSharer = new(() => new ScreenShareClient());
        private static readonly Lazy<ScreenShareServer> s_serverScreenSharer = new(() => new ScreenShareServer());

        public static ScreenShareClient GetScreenShareClient(bool isTesting = false, ICommunicator communicator = null)
        {
            return isTesting ? new ScreenShareClient(communicator) : s_clientScreenSharer.Value;
        }

        public static ScreenShareServer GetScreenShareServer(bool isTesting = false, ICommunicator communicator = null)
        {
            return isTesting ? new ScreenShareServer(communicator) : s_serverScreenSharer.Value;
        }
    }
}